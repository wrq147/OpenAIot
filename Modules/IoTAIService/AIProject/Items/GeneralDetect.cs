using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
using Microsoft.ML.OnnxRuntime.Tensors;
using NPOI.HSSF.Record.CF;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;

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
                        help="请预先生成目标检测特征"
                    },
                    new AIProjectParam() {
                        name="是否绘制检测框",
                        code="if_draw",
                        type="boolean",
                        defval=false,
                        help="是否在视频上绘制检测框"
                    }
                }
            });
        }

        public List<BoxItem> GenerateBoxs(Image<Rgb24> image, AIConfigData config)
        {
            var tparam = new DataDetectParam(config.DetParams);
            float tThreshold = tparam.GetFloat("threshold", 0.2f);
            float tIOU = tparam.GetFloat("iou_threshold", 0.2f);
            string featureTxt = tparam.GetString("feature-txt");
            string featureImg = tparam.GetString("feature-img");
            List<object> feature = tparam.Get("feature") as List<object>;
            List<string> tclasses = null;
            if (!string.IsNullOrEmpty(featureTxt))
            {
                string[] tarr = featureTxt.Split(",");
                tclasses = tarr.ToList();
            }
            if (!string.IsNullOrEmpty(featureImg))
            {
                tclasses = new List<string>();
                tclasses.Add("图片特征");
            }
            var tfeature = ConvertListToDenseTensor(feature);
            var tbbx = _provider.GetService<YoloWorldDetectRunner>().Predict(image, tThreshold, tIOU, tfeature, tclasses);
            List<BoxItem> boxes = new List<BoxItem>();
            foreach (var tbx in tbbx)
            {
                boxes.Add(new BoxItem()
                {
                    x1 = tbx.X1,
                    x2 = tbx.X2,
                    y1 = tbx.Y1,
                    y2 = tbx.Y2,
                    label = tbx.Label,
                    color = "#ff0000",
                    score = tbx.Confidence
                });
            }
            return boxes;
        }
        private DenseTensor<float> ConvertListToDenseTensor(List<object> data)
        {
            // 空数据校验
            if (data == null || data.Count == 0)
            {
                throw new ArgumentException("输入数据不能为空", nameof(data));
            }

            // 获取张量形状：行数（外层列表长度）、列数（第一个内层列表长度）
            int rowCount = data.Count;
            var tmplist = data[0] as List<object>;
            int colCount = tmplist.Count;


            // 展平嵌套列表为一维数组（DenseTensor底层存储格式）
            float[] flatArray = new float[rowCount * colCount];
            int index = 0;
            foreach (var row in data)
            {
                var xxlist = row as List<object>;
                foreach (var value in xxlist)
                {
                    flatArray[index++] = (float)value;
                }
            }

            // 创建DenseTensor：参数1=一维数组，参数2=张量形状
            var tensorShape = new int[] { rowCount, colCount };
            return new DenseTensor<float>(flatArray, tensorShape);
        }
    }
}
