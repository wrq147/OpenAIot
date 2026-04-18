
import torch
import base64
import os
import io
import numpy as np
from PIL import Image
from transformers import (
    AutoImageProcessor,
    AutoTokenizer,
    AutoModelForCausalLM,
)


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


class CNCLIPFeatureExtractor:
    """中文CLIP特征提取器（支持Base64/字节数组输入）"""

    def __init__(self):
        # 设置设备（优先使用GPU）
        self.device = "cuda" if torch.cuda.is_available() else "cpu"

        # 加载模型
        current_dir = os.getcwd()
        model_path = os.path.join(current_dir, "AIScript", "fgmodel")

        try:
            self.fgmodel = AutoModelForCausalLM.from_pretrained(
                model_path, trust_remote_code=True)
            self.tokenizer = AutoTokenizer.from_pretrained(model_path)
            self.image_processor = AutoImageProcessor.from_pretrained(
                model_path)
            self.fgmodel.eval()
            print(f"模型加载成功，使用设备: {self.device}")
        except Exception as e:
            raise RuntimeError(f"模型加载失败: {str(e)}")

    def get_text_features(self, text_list, alignModel):
        """批量提取文本特征"""
        if not isinstance(text_list, list) or len(text_list) == 0:
            raise ValueError("text_list必须是非空字符串列表")

        tmptttt = [t for t in text_list] + [' ']
        token_input = self.tokenizer(
            tmptttt, padding="max_length", max_length=64, truncation=True, return_tensors="pt").to(self.device)

        with torch.no_grad():
            text_features = self.fgmodel.get_text_features(
                **token_input, walk_type="box")
            if alignModel is not None:
                text_features = alignModel(text_features)
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

    def get_image_features_from_base64(self, base64Str, alignModel):
        """
        批量提取Base64图片特征
        :param base64Str: Base64字符串
        :return: 图片特征二维数组
        """

        # 解码Base64为图片
        img = self._decode_base64_to_image(base64Str)
        # 预处理 + 转换为tensor
        img_tensor = self.image_processor(
            images=img, max_num_patches=1600, return_tensors="pt").to(self.device)

        # 批量提取特征
        with torch.no_grad():
            image_features = self.fgmodel.get_image_features(**img_tensor)
            if alignModel is not None:
                image_features = alignModel(image_features)
            image_features = torch.nn.functional.normalize(
                image_features, dim=-1)

        return image_features.cpu().numpy()


global_extractor = CNCLIPFeatureExtractor()
# 外部调用入口（支持Base64/文本输入）


def execall(text_list=None, base64_img=None, projStr="Detect"):
    """
    特征提取统一入口
    :param text_list: 文本列表（可为null）
    :param base64_img: Base64图片字符串（可为null）
    :return: dict 包含text_features/image_features
    """
    if text_list is None and base64_img is None:
        raise ValueError("必须传入text_list或base64_img中的至少一个")

    global global_extractor
    result = {}

    align_model = None
    if projStr == "Detect":
        t_dir = os.getcwd()
        align_path = os.path.join(
            t_dir, "AIScript", "fgclip_to_wedetect_align.pth")
        align_model = FeatureAlignProjection()
        align_model.load_state_dict(torch.load(align_path))
        align_model.eval()

    # 提取文本特征
    if text_list is not None and len(text_list) > 0:
        result["text_features"] = global_extractor.get_text_features(
            text_list, align_model).tolist()

    # 提取Base64图片特征
    if base64_img is not None:
        result["image_features"] = global_extractor.get_image_features_from_base64(
            base64_img, align_model).tolist()

    return result
