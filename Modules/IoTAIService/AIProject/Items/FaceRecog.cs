using ChannelUtility.Message;
using Common;
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
    public class FaceRecog : IInfer
    {
        private ITAServiceProvider _provider;

        public async Task Execute(string deviceId, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
            var tparam = new DataDetectParam(config.DetParams);
            bool tEnableHouse = tparam.GetBool("enable_house");
            var tboxlist = boxes.Where(x => x.label == "人脸").ToList();
            var facenum = tboxlist.Count;
            var aiCache = _provider.GetService<AICache>();
            //开始生成设备属性和事件
            int lastFaceNum = aiCache.GetVideoInt(deviceId, "face_num");
            if (lastFaceNum != facenum)
            {
                //发送人脸数量属性
                Dictionary<string, object> newvals = new Dictionary<string, object>();
                newvals.Add("FaceCount", facenum);
                await _provider.GetService<AIBusProxy>().SendPropertyReply(string.Empty, deviceId, newvals);
                //人脸数量变化事件

            }
            aiCache.SetVideoInt(deviceId, "face_num", facenum);
        }

        public async Task Init(ITAServiceProvider provider)
        {
            _provider = provider;
            string tkey = "FaceRecog";
            provider.GetService<AIProjectManager>().RegInfer(tkey, this);
            var redis = provider.GetService<GeneralRedisHelper>();
            await redis.HashSetAsync("AI-Items", tkey, new AIProjectInfo()
            {
                Name = "人脸识别",
                Code = tkey,
                Stage = "Infer",
                Remark = "人脸识别通过采集人脸图像，提取人脸特征并进行比对，实现快速确认人员身份、精准核验等功能。",
                ParamList = new List<AIProjectParam>()
                {
                    new AIProjectParam()
                    {
                        name="启用人脸库",
                        code="enable_house",
                        type="boolean",
                        help="是否匹配人脸库，并触发相应事件"
                    },
                }
            });
        }
    }
}
