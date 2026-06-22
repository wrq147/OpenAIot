
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

imgsize = 640
featuresize = imgsize // 16
maxnumpatches = featuresize * featuresize


class AdaptedDetectHead(nn.Module):
    def __init__(self):
        super().__init__()
        self.logit_scale = nn.Parameter(torch.ones(1) * 2.6592)
        self.logit_bias = nn.Parameter(torch.zeros(1))

        self.fc = nn.Sequential(
            nn.Linear(768, 768),
            nn.LayerNorm(768),
            nn.GELU(),
            nn.Linear(768, 768),
        )

        self.box_head = nn.Sequential(
            nn.Conv2d(768, 384, kernel_size=3, padding=1),
            nn.BatchNorm2d(384),
            nn.GELU(),
            nn.Conv2d(384, 384, kernel_size=3,
                      padding=1, groups=384),
            nn.BatchNorm2d(384),
            nn.GELU(),
            nn.Conv2d(384, 4, kernel_size=1)
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

    def forward(self, last_hidden, text_feat):
        B = last_hidden.shape[0]
        featsize = int(last_hidden.shape[1] ** 0.5)  # 28
        N = text_feat.size(1)
        fc_text_feat = F.normalize(text_feat, dim=-1)
        fc_img_feat = self.fc(last_hidden)
        fc_img_feat = F.normalize(fc_img_feat, dim=-1)

        # cls_feat: [B,784,768]
        # text_feat: [B,30,768]
        # out:       [B,784,30]
        cls_sim = torch.matmul(fc_img_feat, fc_text_feat.transpose(-1, -2))
        logit_scale = self.logit_scale.exp()
        cls_sim = cls_sim * logit_scale + self.logit_bias
        cls_btm = cls_sim.view(
            B, featsize, featsize, -1).permute(0, 3, 1, 2).contiguous()  # [B,n,28,28]

        max_sim, _ = cls_sim.max(dim=-1)
        confidence = torch.sigmoid(max_sim)
        sim_mean = cls_sim.mean(dim=-1)
        sim_max_smoothed = sim_mean
        sim_max_min = sim_max_smoothed.amin(dim=1, keepdim=True)
        sim_max_max = sim_max_smoothed.amax(dim=1, keepdim=True)
        sim_mean = (sim_max_smoothed - sim_max_min) / \
            (sim_max_max - sim_max_min + 1e-8)
        real_mm = confidence * sim_mean
        mask = real_mm.unsqueeze(-1)          # [B,784,1]
        final_feat = fc_img_feat * mask  # [B, 784, 768]
        final_feat = final_feat.permute(
            0, 2, 1).reshape(B, -1, featsize, featsize)

        box = self.box_head(final_feat)  # [B,4,28,28]
        box = box.flatten(2)           # [B,4,784]

        cls_map = self.cls_head(final_feat)
        cls_map = cls_map.flatten(2)

        return box, cls_map, cls_btm



def resize_and_pad(image, target_size=512):
    w, h = image.size
    scale = target_size / max(w, h)
    new_w = int(w * scale)
    new_h = int(h * scale)
    image = image.resize((new_w, new_h), Image.BILINEAR)
    padded_img = Image.new("RGB", (target_size, target_size), (114, 114, 114))
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
        dethead_path = os.path.join(current_dir, "dethead.pth")
        try:
            self.fgmodel = AutoModelForCausalLM.from_pretrained(
                model_path, trust_remote_code=True)
            self.tokenizer = AutoTokenizer.from_pretrained(model_path)
            self.image_processor = AutoImageProcessor.from_pretrained(
                model_path)
            self.fgmodel.eval()

            self.dethead = AdaptedDetectHead()
            self.dethead.load_state_dict(torch.load(dethead_path, map_location=self.device))
            self.dethead.to(self.device).eval()

            print(f"模型加载成功，使用设备: {self.device}")
        except Exception as e:
            raise RuntimeError(f"模型加载失败: {str(e)}")

    def get_detect_box(self, image_bytes, text_feat, center_thresh=0.8, iou_threshold=0.3):
        raw_img = Image.open(io.BytesIO(image_bytes)).convert("RGB")
        raw_w, raw_h = raw_img.size
        pad_img, pad_x, pad_y, scale = resize_and_pad(raw_img, imgsize)
        image_input = self.image_processor(
            images=pad_img,
            max_num_patches=maxnumpatches,
            return_tensors="pt"
        ).to(self.device)

        with torch.no_grad():
            last_hidden = self.fgmodel.get_vision_feature(**image_input)

            # 检测头前向
            pred_box, pred_cls, pred_sim = self.dethead(last_hidden, text_feat)
            # 解析输出
            pred_box = pred_box.squeeze(0).permute(1, 0)
            pred_sim = pred_sim.flatten(2).permute(0, 2, 1).squeeze(0)

            pred_cls_score = pred_cls.flatten()
            pred_cls_idx = pred_sim.argmax(dim=-1)
            pred_score = torch.sigmoid(pred_cls_score)  # 类别置信度

        results = []
        total_grid = featuresize * featuresize
        for grid_idx in range(total_grid):
            c_score = pred_score[grid_idx].item()

            if c_score < center_thresh:
                continue

            box_tensor = pred_box[grid_idx]
            dx = torch.sigmoid(box_tensor[0])
            dy = torch.sigmoid(box_tensor[1])
            bw = box_tensor[2]
            bh = box_tensor[3]
            w = torch.clamp(torch.exp(bw) * 0.2, 0.0, 1.0)
            h = torch.clamp(torch.exp(bh) * 0.2, 0.0, 1.0)

            gy = grid_idx // featuresize
            gx = grid_idx % featuresize

            cx = (gx + dx * 2 - 0.5)/featuresize
            cy = (gy + dy * 2 - 0.5)/featuresize

            # 映射到 padded 图尺度
            cx_pad = cx * imgsize
            cy_pad = cy * imgsize
            bw_pad = w * imgsize
            bh_pad = h * imgsize

            # 去除padding偏移
            cx_raw = cx_pad - pad_x
            cy_raw = cy_pad - pad_y

            # 还原原始原图尺度
            cx_orig = cx_raw / scale
            cy_orig = cy_raw / scale
            w_orig = bw_pad / scale
            h_orig = bh_pad / scale

            # 转 x1y1x2y2
            x1 = (cx_orig - w_orig / 2).cpu().item()
            y1 = (cy_orig - h_orig / 2).cpu().item()
            x2 = (cx_orig + w_orig / 2).cpu().item()
            y2 = (cy_orig + h_orig / 2).cpu().item()

            x1 = max(0.0, min(x1, raw_w))
            y1 = max(0.0, min(y1, raw_h))
            x2 = max(0.0, min(x2, raw_w))
            y2 = max(0.0, min(y2, raw_h))
            # 类别
            cls_idx = pred_cls_idx[grid_idx]
            results.append({
                "box": [x1, y1, x2, y2],
                "score": c_score,
                "cls_idx": cls_idx.item()
            })

        if len(results) == 0:
            return results

        # 转成 NMS 需要的张量
        boxes = torch.tensor([r["box"] for r in results],
                             dtype=torch.float32).to(self.device)
        scores = torch.tensor([r["score"] for r in results],
                              dtype=torch.float32).to(self.device)

        # 执行 NMS
        keep_idx = nms(boxes[:, :4], scores, iou_threshold=iou_threshold)
        keep_idx = keep_idx.cpu().numpy()

        # 只保留 NMS 后的结果
        results_nms = [results[i] for i in keep_idx]
        return results_nms

    def get_text_features(self, text_list):
        """批量提取文本特征"""
        if not isinstance(text_list, list) or len(text_list) == 0:
            raise ValueError("text_list必须是非空字符串列表")

        tmptttt = [t for t in text_list] + [' ']
        token_input = self.tokenizer(
            tmptttt, padding="max_length", max_length=196, truncation=True, return_tensors="pt").to(self.device)

        with torch.no_grad():
            text_features = self.fgmodel.get_text_features(
                **token_input, walk_type="long")
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
            raw_img = Image.open(io.BytesIO(img_bytes)).convert("RGB")
            pad_img, pad_x, pad_y, scale = resize_and_pad(raw_img, imgsize)
            return pad_img
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
            images=img, max_num_patches=maxnumpatches, return_tensors="pt").to(self.device)

        # 批量提取特征
        with torch.no_grad():
            image_features = self.fgmodel.get_image_features(**img_tensor)

            image_features = torch.nn.functional.normalize(
                image_features, dim=-1)
        return image_features.cpu().numpy()


global_extractor = CNCLIPFeatureExtractor()
# 外部调用入口（支持Base64/文本输入）


def exedetect(image_bytes: bytes, text_feat: List[List[float]], thresh: float, iou_threshold:float) -> List[dict]:
    global global_extractor
    tmp_tensor = torch.tensor(text_feat)
    tmp_tensor = tmp_tensor.unsqueeze(0)
    return global_extractor.get_detect_box(image_bytes, tmp_tensor, thresh,iou_threshold)


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
    final_features = []

    # 提取文本特征
    if text_list is not None and len(text_list) > 0:
        text_feats = global_extractor.get_text_features(
            text_list).tolist()
        final_features.extend(text_feats)

    # 提取Base64图片特征
    if base64_img is not None:
        img_feats = global_extractor.get_image_features_from_base64(
            base64_img).tolist()
        final_features.extend(img_feats)

    return final_features
