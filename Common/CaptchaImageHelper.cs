using Lazy.Captcha.Core;
using Lazy.Captcha.Core.Generator;
using Lazy.Captcha.Core.Generator.Code;
using Lazy.Captcha.Core.Generator.Image;
using Lazy.Captcha.Core.Generator.Image.Option;
using System;
using System.Collections.Generic;
namespace Common
{
    public class CaptchaImageHelper
    {
        public string GetRandomEnDigitalText(int length)
        {
            var _captchaCodeGenerator = new DefaultCaptchaCodeGenerator(CaptchaType.WORD_NUMBER_LOWER);
            var (renderText, code) = _captchaCodeGenerator.Generate(length);
            return code;
        }
        public byte[] GetGifEnDigitalCodeByte(string text)
        {
            var _captchaImageGenerator = new DefaultCaptchaImageGenerator();
            CaptchaImageGeneratorOption option = new CaptchaImageGeneratorOption();
            option.Animation = true; // 是否启用动画
            option.FrameDelay = 10; // 每帧延迟,Animation=true时有效, 默认30
            option.Width = 150; // 验证码宽度
            option.Height = 50; // 验证码高度
            option.BackgroundColor = SixLabors.ImageSharp.Color.White; // 验证码背景色
            option.BubbleCount = 2; // 气泡数量
            option.BubbleMinRadius = 5; // 气泡最小半径
            option.BubbleMaxRadius = 15; // 气泡最大半径
            option.BubbleThickness = 1; // 气泡边沿厚度
            option.InterferenceLineCount = 2; // 干扰线数量
            option.FontSize = 36; // 字体大小
            option.FontFamily = DefaultFontFamilys.Instance.Actionj; // 字体
            option.ForegroundColors = DefaultColors.Instance.Colors;

            var image = _captchaImageGenerator.Generate(text, option);
            return image;
        }
        public byte[] GetEnDigitalCodeByte(string text)
        {
            var _captchaImageGenerator = new DefaultCaptchaImageGenerator();
            CaptchaImageGeneratorOption option = new CaptchaImageGeneratorOption();
            option.Animation = false; // 是否启用动画
            option.Width = 150; // 验证码宽度
            option.Height = 50; // 验证码高度
            option.BackgroundColor = SixLabors.ImageSharp.Color.White; // 验证码背景色
            option.BubbleCount = 2; // 气泡数量
            option.BubbleMinRadius = 5; // 气泡最小半径
            option.BubbleMaxRadius = 15; // 气泡最大半径
            option.BubbleThickness = 1; // 气泡边沿厚度
            option.InterferenceLineCount = 2; // 干扰线数量
            option.FontSize = 36; // 字体大小
            option.FontFamily = DefaultFontFamilys.Instance.Actionj; // 字体
            option.ForegroundColors = DefaultColors.Instance.Colors;
            var image = _captchaImageGenerator.Generate(text, option);
            return image;
        }
    }
}
