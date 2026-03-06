using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
using IoTAIService.Business;
using IoTAIService.DAL;
using Microsoft.ML.OnnxRuntime.Tensors;
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
            var tboxlist = boxes.Where(x => x.label == "人脸").ToList();
            var facenum = tboxlist.Count;
            var aiCache = _provider.GetService<AICache>();
            var videoData = aiCache.GetVideoCache(deviceId);
            int lastFaceNum = videoData.GetInt("face_num", -1);
            if (lastFaceNum != facenum || lastFaceNum == -1)
            {
                //发送人脸数量属性
                Dictionary<string, object> newvals = new Dictionary<string, object>();
                newvals.Add("FaceCount", facenum);
                await _provider.GetService<AIBusProxy>().SendPropertyReply(string.Empty, deviceId, newvals);
            }

            videoData.SetInt("face_num", facenum);
            if (facenum > 0)
            {
                var nowtime = DateTime.Now;
                List<string> hselist = videoData.GetItem<List<string>>("house_list");
                var curtime = videoData.GetDateTime("house_time", nowtime);
                if (hselist == null || (nowtime - curtime).TotalSeconds > 60)
                {
                    var tmplist = await _provider.GetService<AiHouseDAL>().SelectList(x => x.OrgId == config.OrgId);
                    hselist = tmplist.Select(x => x.Id).ToList();
                    videoData.SetDateTime("house_time", nowtime);
                }

                if (hselist.Count > 0)
                {
                    //处理人脸识别事件
                    float tscore = config.GetFloat("facescore", 0.8f);
                    var faceSTNRunner = _provider.GetService<FaceSTNRunner>();
                    var faceRecogRunner = _provider.GetService<FaceRecogRunner>();
                    var milBLL = _provider.GetService<MilvusBLL>();
                    foreach (var titem in tboxlist)
                    {
                        var tmpimg = image.CropByBox(titem.x1, titem.x2, titem.y1, titem.y2);
                        var tmpstn = faceSTNRunner.Predict(tmpimg);
                        var recogdata = faceRecogRunner.PredictTensor(tmpstn);
                        var tmpfls = recogdata.ToArray<float>();
                        var tmprsp = await milBLL.Search(tmpfls, hselist, tscore);
                        if (tmprsp.IsSuccess())
                        {
                            //tlist.AddRange(tmprsp.Data);
                        }
                    }
                }

                //触发陌生人事件

            }

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
                        name="相似度分数",
                        code="facescore",
                        type="float",
                        defval=0.8f,
                        help="人脸库中只有高于这个分数的才分被认定为相似的人脸"
                    }
                }
            });
        }
    }
}
