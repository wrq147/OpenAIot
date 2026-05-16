using ChannelUtility.Message;
using CSnakes.Runtime;
using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService
{
    public class PythonExe
    {
        private ITAServiceProvider _provider;
        public PythonExe(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        public List<BoxItem> GeneratePredictBoxs(byte[] rawBytes, List<List<double>> classEmbeds, List<string> classes, float thresh, float iou_threshold)
        {
            try
            {
                List<BoxItem> tmpboxlist = new List<BoxItem>();
                var py = _provider.GetService<IPythonEnvironment>();
                var outfgclip = py.Outfgclip();

                var output = outfgclip.Exedetect(rawBytes, classEmbeds, thresh, iou_threshold);
                for (int i = 0; i < output.Count; i++)
                {
                    var boxitem = output[i];
                    var clsidx = boxitem.GetAttr("cls_idx").As<int>();
                    var score = boxitem.GetAttr("score").As<float>();
                    var boxs = boxitem.GetAttr("box").As<float[]>();
                    if (clsidx < classes.Count && boxs.Length > 3)
                    {
                        string labelName = classes[clsidx];
                        tmpboxlist.Add(new BoxItem
                        {
                            label = labelName,
                            score = score,
                            x1 = (int)boxs[0],
                            y1 = (int)boxs[1],
                            x2 = (int)boxs[2],
                            y2 = (int)boxs[3],
                            color = "#057C18"
                        });
                    }

                }
                return tmpboxlist;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"调用出错: {ex.Message}");
                // 可以返回null或抛出异常，根据业务需求调整
                throw new InvalidOperationException("Python函数调用失败", ex);
            }
        }
        public List<List<float>> GenerateCNClipFeature(List<string> strArr, string imgStr, string projCode)
        {
            try
            {
                var py = _provider.GetService<IPythonEnvironment>();
                var outfgclip = py.Outfgclip();
                string base64Str = null;
                if (!string.IsNullOrEmpty(imgStr))
                {
                    base64Str = imgStr.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");
                }

                var output = outfgclip.Execall(strArr, base64Str, projCode);
                List<List<float>> rs = output.Select(row => row.Select(x => (float)x).ToList()).ToList();
                if (rs == null)
                {
                    return new List<List<float>>();
                }
                return rs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"调用出错: {ex.Message}");
                // 可以返回null或抛出异常，根据业务需求调整
                throw new InvalidOperationException("Python函数调用失败", ex);
            }
        }


    }
}
