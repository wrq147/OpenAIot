using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
using Microsoft.ML.OnnxRuntime.Tensors;
using NPOI.HPSF;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.AIProject.Items
{
    /// <summary>
    /// 通用检测项目
    /// </summary>
    public class GeneralDetect : IDetect
    {
        private ITAServiceProvider _provider;
        public async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "Detect";
            provider.GetService<AIProjectManager>().RegDetect(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "通用检测",
                Code = tkey,
                Stage = "Detect",
                Remark = "通用检测能够根据描述性文本检测图像中的任何物体。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="检测阈值",
                        code="threshold",
                        type="float",
                        defval=0.2f,
                        min=0,
                        max=1,
                        help="0~1的区间值,值越小,对目标检测越模糊"
                    },
                    new AIProjectParam()
                    {
                        name="交并阈值",
                        code="iou_threshold",
                        type="float",
                        defval=0.2f,
                        min=0,
                        max=1,
                        help="0~1的区间值,值越小,越不会检测重合目标"
                    },
                    new AIProjectParam()
                    {
                        name="目标特征",
                        code="feature",
                        type="clip",
                        help="必填项,用来生成目标检测特征"
                    }
                }
            });
        }

        public List<BoxItem> GenerateBoxs(Image<Rgb24> image, byte[] rawBytes, AIConfigData config)
        {
            float tThreshold = config.GetFloat("threshold", 0.2f);
            float tIOU = config.GetFloat("iou_threshold", 0.2f);
            string featureTxt = config.GetString("feature-txt");
            string featureImg = config.GetString("feature-img");
            List<object> feature = config.Get("feature") as List<object>;
            List<string> tclasses = null;
            if (!string.IsNullOrEmpty(featureTxt))
            {
                string[] tarr = featureTxt.Split(",");
                tclasses = tarr.ToList();
            }

            var tfeature = ConvertListToFloatList(feature);
            if (!string.IsNullOrEmpty(featureImg))
            {
                tclasses = new List<string>();
                tclasses.Add("图片对象");
            }
            if (tclasses == null)
            {
                return new List<BoxItem>();
            }
            PythonExe py = _provider.GetService<PythonExe>();
            List<BoxItem> boxes = py.GeneratePredictBoxs(rawBytes, tfeature, tclasses, tThreshold, tIOU);
            return boxes;
        }

        private List<List<double>> ConvertListToFloatList(List<object> data)
        {
            // 空数据校验
            if (data == null || data.Count == 0)
            {
                throw new ArgumentException("输入数据不能为空", nameof(data));
            }

            // 初始化结果集合
            List<List<double>> result = new List<List<double>>();

            // 遍历每一行
            foreach (var rowObj in data)
            {
                // 把外层 object 强转为内层 List<object>
                if (rowObj is not List<object> rowList)
                {
                    throw new InvalidCastException("数据格式错误，每行必须是 List<object> 类型");
                }

                // 转换当前行：object → float
                List<double> floatRow = new List<double>();
                foreach (var value in rowList)
                {
                    floatRow.Add(Convert.ToDouble(value));
                }

                // 添加到结果
                result.Add(floatRow);
            }

            return result;
        }
        //private DenseTensor<float> ConvertListToDenseTensor(List<object> data)
        //{
        //    // 空数据校验
        //    if (data == null || data.Count == 0)
        //    {
        //        throw new ArgumentException("输入数据不能为空", nameof(data));
        //    }

        //    // 获取张量形状：行数（外层列表长度）、列数（第一个内层列表长度）
        //    int rowCount = data.Count;
        //    var tmplist = data[0] as List<object>;
        //    int colCount = tmplist.Count;


        //    // 计算总元素数 = 批次 × 行数 × 列数
        //    int totalElements = 1 * rowCount * colCount;
        //    float[] flatArray = new float[totalElements];
        //    int index = 0;
        //    foreach (var row in data)
        //    {
        //        var xxlist = row as List<object>;
        //        foreach (var value in xxlist)
        //        {
        //            var tt = value.GetType();
        //            flatArray[index++] = Convert.ToSingle(value);
        //        }
        //    }

        //    // ---------------------- 第三步：创建三维DenseTensor ----------------------
        //    // 张量形状：[batch, rowCount, colCount]（对应batch/文本长度/嵌入维度）
        //    var tensorShape = new int[] { 1, rowCount, colCount };
        //    return new DenseTensor<float>(flatArray, tensorShape);
        //}
    }
}
