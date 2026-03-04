import torch
import cnclip
import base64
import os
import numpy as np
from PIL import Image
from cnclip import load_from_name


class CNCLIPFeatureExtractor:
    """中文CLIP特征提取器（支持Base64/字节数组输入）"""

    def __init__(self):
        # 设置设备（优先使用GPU）
        self.device = "cuda" if torch.cuda.is_available() else "cpu"

        # 加载模型
        current_dir = os.getcwd()
        model_path = os.path.join(
            current_dir, "AIScript", "clip_cn_vit-b-16.pt")

        try:
            self.model, self.preprocess = load_from_name(
                model_path,
                device=self.device,
                vision_model_name="ViT-B-16",
                text_model_name="RoBERTa-wwm-ext-base-chinese",
                input_resolution=224
            )
            self.model.eval()
            print(f"模型加载成功，使用设备: {self.device}")
        except Exception as e:
            raise RuntimeError(f"模型加载失败: {str(e)}")

    def get_text_features(self, text_list):
        """批量提取文本特征"""
        if not isinstance(text_list, list) or len(text_list) == 0:
            raise ValueError("text_list必须是非空字符串列表")

        text = cnclip.tokenize(text_list).to(self.device)
        with torch.no_grad():
            text_features = self.model.encode_text(text)
            text_features /= text_features.norm(p=2, dim=-1, keepdim=True)

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
            img = Image.open(np.io.BytesIO(img_bytes)).convert("RGB")
            return img
        except Exception as e:
            raise RuntimeError(f"Base64图片解码失败: {str(e)}")

    def get_image_features_from_base64(self, base64_list):
        """
        批量提取Base64图片特征
        :param base64_list: Base64字符串列表
        :return: 图片特征二维数组
        """
        if not isinstance(base64_list, list) or len(base64_list) == 0:
            raise ValueError("base64_list必须是非空字符串列表")

        image_tensors = []
        for base64_str in base64_list:
            # 解码Base64为图片
            img = self._decode_base64_to_image(base64_str)
            # 预处理 + 转换为tensor
            img_tensor = self.preprocess(img).to(self.device)
            image_tensors.append(img_tensor)

        # 批量提取特征
        batch_images = torch.stack(image_tensors)
        with torch.no_grad():
            image_features = self.model.encode_image(batch_images)
            image_features /= image_features.norm(p=2, dim=-1, keepdim=True)

        return image_features.cpu().numpy()


global_extractor = CNCLIPFeatureExtractor()
# 外部调用入口（支持Base64/文本输入）


def execall(text_list=None, base64_img_list=None):
    """
    特征提取统一入口
    :param text_list: 文本列表（可为null）
    :param base64_img_list: Base64图片字符串列表（可为null）
    :return: dict 包含text_features/image_features
    """
    if text_list is None and base64_img_list is None:
        raise ValueError("必须传入text_list或base64_img_list中的至少一个")

    global global_extractor
    result = {}

    # 提取文本特征
    if text_list is not None and len(text_list) > 0:
        result["text_features"] = global_extractor.get_text_features(
            text_list).tolist()

    # 提取Base64图片特征
    if base64_img_list is not None and len(base64_img_list) > 0:
        result["image_features"] = global_extractor.get_image_features_from_base64(
            base64_img_list).tolist()

    return result
