using Common;
using Common.Share;
using DeveloperService;
using DeveloperService.Model;
using IoTAIService.AICode;
using IoTAIService.Business;
using IoTAIService.Models;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using SixLabors.ImageSharp;
using System.Linq;

namespace IoTAIService.Controller
{
    public class HttpSync : AbstractDeveloperController
    {
        private MZ_Developer _develper;

        private ITAServiceProvider _provider;
        public HttpSync(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        /// <summary>
        /// 校验开发者权限
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task<IResult> CallAction(TAAction ac, object[] parameters)
        {
            _develper = GetDeveloper();
            if (_develper.UserType != 1)
            {
                return this.Error<string>(11, "必需为企业开发者");
            }
            return await base.CallAction(ac, parameters);
        }


        private static readonly List<Out_FaceBox> EmptyFaceBox = new List<Out_FaceBox>();
        /// <summary>
        /// 检测人脸
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<DefaultAjaxResult<List<Out_FaceBox>>> FaceDetect(In_FaceDetect data)
        {
            if (!string.IsNullOrEmpty(data.ImageBase64))
            {
                string base64Data = data.ImageBase64;
                if (base64Data.Contains(','))
                {
                    base64Data = base64Data.Split(',')[1]; // 取逗号后的纯 Base64 部分
                }

                try
                {
                    // Base64 解码为字节数组
                    byte[] imageBytes = Convert.FromBase64String(base64Data);
                    List<Out_FaceBox> tlist = new List<Out_FaceBox>();
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        var originalImage = Image.Load<Rgb24>(ms);
                        var tbbx = _provider.GetService<FaceDetOnnxRunner>().Predict(originalImage, data.Threshold, data.IOU_Threshold);
                        foreach (var titem in tbbx)
                        {
                            tlist.Add(new Out_FaceBox()
                            {
                                X1 = titem.X1,
                                X2 = titem.X2,
                                Y1 = titem.Y1,
                                Y2 = titem.Y2,
                                Score = titem.Score
                            });
                        }
                    }
                    return this.Success(tlist);
                }
                catch (FormatException ex)
                {
                    return this.Error(13, "无效的 Base64 格式", EmptyFaceBox);
                }
                catch (Exception ex)
                {
                    return this.Error(14, "转换图像失败", EmptyFaceBox);
                }
            }
            else if (!string.IsNullOrEmpty(data.ImageUrl))
            {
                List<Out_FaceBox> tlist = new List<Out_FaceBox>();
                var originalImage = await _provider.GetService<FileHelper>().CreateRgb24FromUrl(data.ImageUrl);
                var tbbx = _provider.GetService<FaceDetOnnxRunner>().Predict(originalImage, data.Threshold, data.IOU_Threshold);
                foreach (var titem in tbbx)
                {
                    tlist.Add(new Out_FaceBox()
                    {
                        X1 = titem.X1,
                        X2 = titem.X2,
                        Y1 = titem.Y1,
                        Y2 = titem.Y2,
                        Score = titem.Score
                    });
                }
                return this.Success(tlist);
            }
            else
            {
                return this.Error(12, "Base64和Url不能都为空", EmptyFaceBox);
            }
        }
        private static readonly List<long> EmptyFaceFeature = new List<long>();
        /// <summary>
        /// 识别人脸
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<DefaultAjaxResult<List<long>>> FaceRecog(In_FaceRecog data)
        {
            if (!string.IsNullOrEmpty(data.ImageBase64))
            {
                string base64Data = data.ImageBase64;
                if (base64Data.Contains(','))
                {
                    base64Data = base64Data.Split(',')[1]; // 取逗号后的纯 Base64 部分
                }

                try
                {
                    // Base64 解码为字节数组
                    byte[] imageBytes = Convert.FromBase64String(base64Data);
                    List<long> tlist = new List<long>();
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        var originalImage = Image.Load<Rgb24>(ms);
                        var tbbx = _provider.GetService<FaceDetOnnxRunner>().Predict(originalImage, data.Threshold, data.IOU_Threshold);
                        var faceSTNRunner = _provider.GetService<FaceSTNRunner>();
                        var faceRecogRunner = _provider.GetService<FaceRecogRunner>();
                        var milBLL = _provider.GetService<MilvusBLL>();
                        foreach (var titem in tbbx)
                        {
                            var tmpimg = originalImage.CropByBox(titem.X1, titem.X2, titem.Y1, titem.Y2);
                            var tmpstn = faceSTNRunner.Predict(tmpimg);
                            var recogdata = faceRecogRunner.PredictTensor(tmpstn);
                            var tmpfls = recogdata.ToArray<float>();
                            var tmprsp = await milBLL.Search(tmpfls, data.HouseId);
                            if (tmprsp.IsSuccess())
                            {
                                tlist.AddRange(tmprsp.Data);
                            }
                        }
                    }
                    return this.Success(tlist);
                }
                catch (FormatException)
                {
                    return this.Error(13, "无效的 Base64 格式", EmptyFaceFeature);
                }
                catch (Exception)
                {
                    return this.Error(14, "转换图像失败", EmptyFaceFeature);
                }
            }
            else if (!string.IsNullOrEmpty(data.ImageUrl))
            {
                List<long> tlist = new List<long>();

                var originalImage = await _provider.GetService<FileHelper>().CreateRgb24FromUrl(data.ImageUrl);
                var tbbx = _provider.GetService<FaceDetOnnxRunner>().Predict(originalImage, data.Threshold, data.IOU_Threshold);
                var faceSTNRunner = _provider.GetService<FaceSTNRunner>();
                var faceRecogRunner = _provider.GetService<FaceRecogRunner>();
                var milBLL = _provider.GetService<MilvusBLL>();
                foreach (var titem in tbbx)
                {
                    var tmpimg = originalImage.CropByBox(titem.X1, titem.X2, titem.Y1, titem.Y2);
                    Tensor<float> recogdata;
                    if (data.EnableSTN == true)
                    {
                        var tmpstn = faceSTNRunner.Predict(tmpimg);
                        recogdata = faceRecogRunner.PredictTensor(tmpstn);
                    }
                    else
                    {
                        recogdata = faceRecogRunner.Predict(tmpimg);
                    }
                    var tmpfls = recogdata.ToArray<float>();
                    var tmprsp = await milBLL.Search(tmpfls, data.HouseId);
                    if (tmprsp.IsSuccess())
                    {
                        tlist.AddRange(tmprsp.Data);
                    }
                }
                return this.Success(tlist);
            }
            else
            {
                return this.Error(12, "Base64和Url不能都为空", EmptyFaceFeature);
            }
        }

    }
}
