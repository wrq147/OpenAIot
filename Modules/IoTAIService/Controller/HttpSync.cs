using AuthService;
using Common;
using Common.Share;
using DeveloperService;
using DeveloperService.Model;
using IoTAIService.AICode;
using IoTAIService.Business;
using IoTAIService.DAL;
using IoTAIService.Models;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;

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
                    using (var originalImage = Image.Load<Rgb24>(imageBytes))
                    {
                        var tbbx = _provider.GetService<YoloFaceDetectRunner>().Predict(originalImage, data.Threshold, data.IOU_Threshold);
                        foreach (var titem in tbbx)
                        {
                            tlist.Add(new Out_FaceBox()
                            {
                                X1 = titem.X1,
                                X2 = titem.X2,
                                Y1 = titem.Y1,
                                Y2 = titem.Y2,
                                Score = titem.Confidence
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
                var tbbx = _provider.GetService<YoloFaceDetectRunner>().Predict(originalImage, data.Threshold, data.IOU_Threshold);
                foreach (var titem in tbbx)
                {
                    tlist.Add(new Out_FaceBox()
                    {
                        X1 = titem.X1,
                        X2 = titem.X2,
                        Y1 = titem.Y1,
                        Y2 = titem.Y2,
                        Score = titem.Confidence
                    });
                }
                return this.Success(tlist);
            }
            else
            {
                return this.Error(12, "Base64和Url不能都为空", EmptyFaceBox);
            }
        }

        /// <summary>
        /// 识别人脸
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<DefaultAjaxResult<Out_FaceMem[]>> FaceRecog(In_FaceRecog data)
        {
            var houseList = await _provider.GetService<AiHouseDAL>().SelectList(x => x.OrgId == _develper.OrgId && x.Status == "1" && x.HouseName == data.HouseName);
            if (houseList == null || houseList.Count == 0)
            {
                return this.Error(20, "人脸库名称被禁用或不存在", Array.Empty<Out_FaceMem>());
            }
            List<long> tlist = new List<long>();
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
                    using (var originalImage = Image.Load<Rgb24>(imageBytes))
                    {
                        var tbbx = _provider.GetService<YoloFaceDetectRunner>().Predict(originalImage, data.Threshold, data.IOU_Threshold);
                        var faceSTNRunner = _provider.GetService<FaceSTNRunner>();
                        var faceRecogRunner = _provider.GetService<FaceRecogRunner>();
                        var milBLL = _provider.GetService<MilvusBLL>();
                        foreach (var titem in tbbx)
                        {
                            var tmpimg = originalImage.CropByBox(titem.X1, titem.X2, titem.Y1, titem.Y2);
                            var tmpstn = faceSTNRunner.Predict(tmpimg);
                            var recogdata = faceRecogRunner.PredictTensor(tmpstn);
                            var tmpfls = recogdata.ToArray<float>();
                            var tmprsp = await milBLL.Search(tmpfls, houseList[0].Id);
                            if (tmprsp.IsSuccess())
                            {
                                tlist.AddRange(tmprsp.Data);
                            }
                        }
                    }
                }
                catch (FormatException)
                {
                    return this.Error(13, "无效的 Base64 格式", Array.Empty<Out_FaceMem>());
                }
                catch (Exception)
                {
                    return this.Error(14, "转换图像失败", Array.Empty<Out_FaceMem>());
                }
            }
            else if (!string.IsNullOrEmpty(data.ImageUrl))
            {

                var originalImage = await _provider.GetService<FileHelper>().CreateRgb24FromUrl(data.ImageUrl);
                var tbbx = _provider.GetService<YoloFaceDetectRunner>().Predict(originalImage, data.Threshold, data.IOU_Threshold);
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
                    var tmprsp = await milBLL.Search(tmpfls, houseList[0].Id);
                    if (tmprsp.IsSuccess())
                    {
                        tlist.AddRange(tmprsp.Data);
                    }
                }
            }
            else
            {
                return this.Error(12, "Base64和Url不能都为空", Array.Empty<Out_FaceMem>());
            }

            if (tlist.Count > 0)
            {
                var tmpuserlist = await _provider.GetService<UserDAL>().GetUserListByIds(tlist);
                Out_FaceMem[] tmemArr = new Out_FaceMem[tmpuserlist.Count];
                for (int i = 0; i < tmemArr.Length; i++)
                {
                    tmemArr[i] = new Out_FaceMem()
                    {
                        UserId = tmemArr[i].UserId,
                        RealName = tmemArr[i].RealName
                    };
                }
                return this.Success(tmemArr);
            }
            else
            {
                return this.Success(Array.Empty<Out_FaceMem>());
            }
        }

    }
}
