
import torch
import torch.nn as nn
import torch.nn.functional as F
from torchvision.ops import nms
import base64
import os
import io
from PIL import Image
from transformers import (
    AutoImageProcessor,
    AutoTokenizer,
    AutoModelForCausalLM,
)
from typing import List


class AdaptedDetectHead(nn.Module):
    def __init__(self, hidden_dim=384):
        super().__init__()
        self.hidden_dim = hidden_dim
        self.scale = nn.Parameter(torch.ones(1) * 0.5)

        self.box_head = nn.Sequential(
            nn.Conv2d(768, hidden_dim, kernel_size=3, padding=1),
            nn.BatchNorm2d(hidden_dim),
            nn.GELU(),
            nn.Conv2d(hidden_dim, hidden_dim, kernel_size=3,
                      padding=1, groups=hidden_dim),
            nn.BatchNorm2d(hidden_dim),
            nn.GELU(),
            nn.Conv2d(hidden_dim, 4, kernel_size=1)
        )

        self.cls_head = nn.Sequential(
            nn.Conv2d(768, 192, kernel_size=3, padding=1),
            nn.BatchNorm2d(192),
            nn.GELU(),
            nn.Conv2d(192, 192, kernel_size=3, padding=1, groups=192),
            nn.BatchNorm2d(192),
            nn.GELU(),
            nn.Conv2d(192, 1, kernel_size=1)
        )

        gauss_kernel = torch.tensor([
            [1.0, 2.0, 1.0],
            [2.0, 4.0, 2.0],
            [1.0, 2.0, 1.0]
        ]) / 16.0
        self.register_buffer('gauss_kernel', gauss_kernel.view(1, 1, 3, 3))

    def forward(self, last_hidden, dense_feat, text_feat):
        B = last_hidden.shape[0]
        featsize = int(last_hidden.shape[1] ** 0.5)  # 28
        N = text_feat.size(1)

        text_feat = F.normalize(text_feat, dim=-1)
        img_dense_feat = F.normalize(dense_feat, dim=-1)

        # [B, 784, 768] -> [B, 768, 28, 28]
        img_feat = last_hidden.view(
            B, featsize, featsize, -1).permute(0, 3, 1, 2).contiguous()

        # -------------------------- 框回归 --------------------------
        box = self.box_head(img_feat)  # [B,4,28,28]
        box = box.flatten(2)           # [B,4,784]

        # -------------------------- 核心：einsum 批量相似度 --------------------------

        # cls_feat: [B,784,768]
        # text_feat: [B,30,768]
        # out:       [B,784,30]
        cls_sim = torch.matmul(img_dense_feat, text_feat.transpose(-1, -2))
        cls_sim = cls_sim / self.scale
        cls_btm = cls_sim.view(
            # [B,n,28,28]
            B, featsize, featsize, -1).permute(0, 3, 1, 2).contiguous()
        sim_max, _ = cls_sim.max(dim=-1)  # [B,784]

        sim_map = sim_max.view(B, 1, featsize, featsize)
        sim_smooth = F.conv2d(
            sim_map, self.gauss_kernel.to(sim_map.device), padding=1)
        sim_max_smoothed = sim_smooth.flatten(1)

        sim_max_min = sim_max_smoothed.amin(dim=1, keepdim=True)
        sim_max_max = sim_max_smoothed.amax(dim=1, keepdim=True)
        sim_max = (sim_max_smoothed - sim_max_min) / \
            (sim_max_max - sim_max_min + 1e-8)

        mask = sim_max.unsqueeze(-1)          # [B,784,1]

        final_feat = img_dense_feat * mask  # [B, 784, 768]
        final_feat = final_feat.permute(
            0, 2, 1).reshape(B, -1, featsize, featsize)

        cls_map = self.cls_head(final_feat)
        cls_map = cls_map.flatten(2)

        return box, cls_map, cls_btm


class FeatureAlignProjection(nn.Module):
    def __init__(self, in_dim=768, out_dim=768, hidden_dim=1024):
        super().__init__()
        # 🔥 二层 MLP + 归一化
        self.fc1 = nn.Linear(in_dim, hidden_dim)
        self.norm1 = nn.LayerNorm(hidden_dim)
        self.act = nn.GELU()  # 比ReLU更平滑

        self.fc2 = nn.Linear(hidden_dim, out_dim, bias=True)

    def forward(self, x):
        # 第一层
        x = self.fc1(x)
        x = self.norm1(x)
        x = self.act(x)

        # 第二层
        x = self.fc2(x)
        return x


def resize_short_edge(image, target_size=640):
    if isinstance(image, str):
        image = Image.open(image)
    width, height = image.size
    short_edge = min(width, height)

    if short_edge >= target_size:
        return image
    scale = target_size / short_edge
    new_width = int(width * scale)
    new_height = int(height * scale)
    resized_image = image.resize((new_width, new_height))
    return resized_image


def resize_and_pad(image, target_size=512, fill_color=(114, 114, 114)):
    w, h = image.size
    scale = target_size / max(w, h)
    new_w = int(w * scale)
    new_h = int(h * scale)
    image = image.resize((new_w, new_h), Image.BILINEAR)
    padded_img = Image.new("RGB", (target_size, target_size), fill_color)
    paste_x = (target_size - new_w) // 2
    paste_y = (target_size - new_h) // 2
    padded_img.paste(image, (paste_x, paste_y))
    return padded_img, paste_x, paste_y, scale


class CNCLIPFeatureExtractor:
    """中文CLIP特征提取器（支持Base64/字节数组输入）"""

    def __init__(self):
        # 设置设备（优先使用GPU）
        self.device = "cuda" if torch.cuda.is_available() else "cpu"

        # 加载模型
        current_dir = os.path.dirname(os.path.abspath(__file__))
        model_path = os.path.join(current_dir, "fgmodel")

        try:
            self.fgmodel = AutoModelForCausalLM.from_pretrained(
                model_path, trust_remote_code=True)
            self.tokenizer = AutoTokenizer.from_pretrained(model_path)
            self.image_processor = AutoImageProcessor.from_pretrained(
                model_path)
            self.fgmodel.eval()

            self.dethead = AdaptedDetectHead()
            self.dethead.load_state_dict(torch.load(
                "dethead_yolo_best.pth", map_location=self.device))
            self.dethead.to(self.device).eval()

            print(f"模型加载成功，使用设备: {self.device}")
        except Exception as e:
            raise RuntimeError(f"模型加载失败: {str(e)}")

    def get_detect_box(self, image_bytes, class_names, imgsize=512):
        raw_img = Image.open(io.BytesIO(image_bytes)).convert("RGB")
        pad_img, pad_x, pad_y, scale = resize_and_pad(raw_img, imgsize)

    def get_text_features(self, text_list):
        """批量提取文本特征"""
        if not isinstance(text_list, list) or len(text_list) == 0:
            raise ValueError("text_list必须是非空字符串列表")

        tmptttt = [t for t in text_list] + [' ']
        token_input = self.tokenizer(
            tmptttt, padding="max_length", max_length=64, truncation=True, return_tensors="pt").to(self.device)

        with torch.no_grad():
            text_features = self.fgmodel.get_text_features(
                **token_input, walk_type="box")
            text_features = torch.nn.functional.normalize(
                text_features, dim=-1)

        return text_features.cpu().numpy()

    def _decode_base64_to_image(self, base64_str):
        """
        将Base64字符串解码为PIL Image
        :param base64_str: 纯Base64字符串（不含data:image/*;base64,前缀）
        :return: PIL Image对象
        """
        try:
            # 解码Base64为字节数据
            img_bytes = base64.b64decode(base64_str)
            # 从字节数据加载图片（自动识别尺寸）
            img = Image.open(io.BytesIO(img_bytes)).convert("RGB")
            return resize_short_edge(img)
        except Exception as e:
            raise RuntimeError(f"Base64图片解码失败: {str(e)}")

    def get_image_features_from_base64(self, base64Str):
        """
        批量提取Base64图片特征
        :param base64Str: Base64字符串
        :return: 图片特征二维数组
        """

        # 解码Base64为图片
        img = self._decode_base64_to_image(base64Str)
        # 预处理 + 转换为tensor
        img_tensor = self.image_processor(
            images=img, max_num_patches=256, return_tensors="pt").to(self.device)

        # 批量提取特征
        with torch.no_grad():
            image_features = self.fgmodel.get_image_features(**img_tensor)

            image_features = torch.nn.functional.normalize(
                image_features, dim=-1)
        return image_features.cpu().numpy()


global_extractor = CNCLIPFeatureExtractor()
# 外部调用入口（支持Base64/文本输入）


def execall(
    text_list: List[str],
    base64_img: str,
    projStr: str
) -> List[List[float]]:
    """
    特征提取统一入口
    :param text_list: 文本列表（可为null）
    :param base64_img: Base64图片字符串（可为null）
    :param projStr: 项目代码
    :param enableCosCluster: 是否使用余弦相似度聚类
    :return: dict 包含text_features/image_features
    """
    if text_list is None and base64_img is None:
        raise ValueError("必须传入text_list或base64_img中的至少一个")

    global global_extractor
    result = {}

    # 提取文本特征
    if text_list is not None and len(text_list) > 0:
        result["text_features"] = global_extractor.get_text_features(
            text_list).tolist()

    # 提取Base64图片特征
    if base64_img is not None:
        result["image_features"] = global_extractor.get_image_features_from_base64(
            base64_img).tolist()

    return result
