
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Common
{
    public class ImageHelper
    {
        private Image srcImage;
        private bool _textMark = true;
        private bool _imgMark = false;
        private string _text = "水印文本";
        private string _imgPath = "";
        private int _markX = 0;
        private int _markY = 0;
        private float _transparency = 1;
        private string _fontFamily = "宋体";
        private Color _textColor = Color.Black;
        private bool _textbold = false;
        int[] sizes = new int[] { 48, 32, 16, 8, 6, 4 };

        public int GetWidth()
        {
            return srcImage.Width;
        }
        public int GetHeight()
        {
            return srcImage.Height;
        }
        /// <summary>
        /// 是否添加文字水印
        /// </summary>
        public bool TextMark
        {
            get { return _textMark; }
            set { _textMark = value; }
        }
        /// <summary>
        /// 是否添加图片水印
        /// </summary>
        public bool ImageMark
        {
            get { return _imgMark; }
            set { _imgMark = value; }
        }
        /// <summary>
        /// 文字水印得内容
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        /// <summary>
        /// 图片水印得图片地址
        /// </summary>
        public string ImagePath
        {
            get { return _imgPath; }
            set { _imgPath = value; }
        }
        /// <summary>
        /// 添加水印位置得横坐标
        /// </summary>
        public int MarkX
        {
            get { return _markX; }
            set { _markX = value; }
        }
        /// <summary>
        /// 添加水印位置得纵坐标
        /// </summary>
        public int MarkY
        {
            get { return _markY; }
            set { _markY = value; }
        }
        /// <summary>
        /// 水印得透明度
        /// </summary>
        public float Transparency
        {
            get
            {
                if (_transparency > 1.0f)
                {
                    _transparency = 1.0f;
                }
                return _transparency;
            }
            set { _transparency = value; }
        }
        /// <summary>
        /// 水印文字得颜色
        /// </summary>
        public Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }
        /// <summary>
        /// 水印文字得字体
        /// </summary>
        public string TextFontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }
        /// <summary>
        /// 水印文字是否加粗
        /// </summary>
        public bool Bold
        {
            get { return _textbold; }
            set { _textbold = value; }
        }


        public ImageHelper(string fileName)
        {
            srcImage = Image.Load(fileName);
        }
        public ImageHelper(Stream stream)
        {
            srcImage = Image.Load(stream);
        }



        /// <summary>
        /// 压缩保存
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task CompressSaveAsync(int width, int height, string path)
        {
            Image _thumb = this.MakeThumbnailImage(width, height);
            await _thumb.SaveAsync(path);
        }



        /// <summary>
        /// 生成水印
        /// </summary>
        /// <returns></returns>
        public Image GetWaterMark()
        {
            try
            {
                //添加文字水印
                if (this.TextMark)
                {
                    srcImage.Mutate(processingContext =>
                    {
                        Size imgSize = processingContext.GetCurrentSize();
                        int padding = 5;
                        float targetWidth = imgSize.Width - (padding * 2);
                        float targetHeight = imgSize.Height - (padding * 2);

                        Font font = null;
                        //探测出一个适合图片大小得字体大小，以适应水印文字大小得自适应
                        for (int i = 0; i < 6; i++)
                        {
                            //是否加粗
                            if (!this.Bold)
                            {
                                font = SystemFonts.CreateFont(this.TextFontFamily, sizes[i], FontStyle.Regular);
                            }
                            else
                            {
                                font = SystemFonts.CreateFont(this.TextFontFamily, sizes[i], FontStyle.Bold);
                            }

                            TextOptions txtOption = new TextOptions(font);
                            FontRectangle size = TextMeasurer.MeasureSize(this.Text, txtOption);
                            //匹配第一个符合要求得字体大小
                            if ((ushort)size.Width < (ushort)srcImage.Width)
                            {
                                break;
                            }
                        }

                        var textGraphicOptions = new RichTextOptions(font)
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Origin = new System.Numerics.Vector2(imgSize.Width / 2, imgSize.Height / 2)
                        };
                        processingContext.DrawText(textGraphicOptions, this.Text, this.TextColor);
                    });
                    return srcImage;
                }
                //添加图像水印
                if (this.ImageMark)
                {
                    //获得水印图像
                    Image markImg = Image.Load(this.ImagePath);
                    srcImage.Mutate(processingContext =>
                    {
                        //如果原图过小
                        if (markImg.Width > srcImage.Width || markImg.Height > srcImage.Height)
                        {
                            markImg = MakeThumbnailImage(markImg, srcImage.Width / 4, markImg.Height * srcImage.Width / markImg.Width);
                        }
                        //添加水印
                        processingContext.DrawImage(markImg, new Point(this.MarkX, this.MarkY), PixelColorBlendingMode.Multiply, 0.3f);
                    });
                    return srcImage;
                }
                return srcImage;
            }
            catch
            {
                return srcImage;
            }
        }
        public void Dispose()
        {
            srcImage.Dispose();
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Image MakeThumbnailImage(int width, int height)
        {
            return MakeThumbnailImage(this.srcImage, width, height);
        }

        private Image MakeThumbnailImage(Image img, int width, int height)
        {
            int towidth = width;
            int toheight = height;

            int ow = srcImage.Width;
            int oh = srcImage.Height;

            float ratio = 1;
            if (srcImage.Width > srcImage.Height)
            {
                ratio = Math.Min(1.0f, (float)toheight / srcImage.Height);
            }
            else
            {
                ratio = Math.Min(1.0f, (float)towidth / srcImage.Width);
            }

            toheight = (int)(srcImage.Height * ratio);
            towidth = (int)(srcImage.Width * ratio);

            img.Mutate(x => x
             .Resize(towidth, toheight));

            return img;
        }



    }
}
