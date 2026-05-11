using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
using IoTAIService.Business;
using IoTAIService.DAL;
using IoTAIService.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.AIProject.Items
{
    public class FaceRecog : Infer
    {
        private ITAServiceProvider _provider;

        public override async Task Execute(AIDetectRequestMeesage req, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
            var aiCache = _provider.GetService<AICache>();
            var videoData = aiCache.GetVideoCache(req.DeviceId);
            var tracklist = videoData.TrackList.Where(x => x.CurrentDetection.label == "人脸").ToList();
            var addlist = videoData.AddTrackList.Where(x => x.CurrentDetection.label == "人脸").ToList();

            var facenum = tracklist.Count;
            int lastFaceNum = videoData.GetInt("face_num", -1);
            var aiBusProxy = _provider.GetService<AIBusProxy>();
            if (lastFaceNum != facenum || lastFaceNum == -1)
            {
                //发送人脸数量属性
                Dictionary<string, object> newvals = new Dictionary<string, object>();
                newvals.Add("FaceCount", facenum);
                await aiBusProxy.SendPropertyReply(string.Empty, req.DeviceId, newvals);
            }

            videoData.SetInt("face_num", facenum);

            if (addlist.Count > 0)
            {
                var nowtime = DateTime.Now;
                var tmplist = await _provider.GetService<AiHouseDAL>().SelectList(x => x.OrgId == config.OrgId);
                List<string> hselist = tmplist.Select(x => x.Id).ToList();

                //处理人脸识别事件
                float tscore = config.GetFloat("facescore", 0.8f);
                var faceSTNRunner = _provider.GetService<FaceSTNRunner>();
                var faceRecogRunner = _provider.GetService<FaceRecogRunner>();
                var milBLL = _provider.GetService<MilvusBLL>();


                List<Out_MemHouse> addMemList = new List<Out_MemHouse>();
                List<Image<Rgb24>> knowList = new List<Image<Rgb24>>();
                List<Image<Rgb24>> unknowList = new List<Image<Rgb24>>();
                var addfaces = addlist.Select(x => x.CurrentDetection);
                foreach (var titem in addfaces)
                {
                    var tmpimg = image.CropByBox(titem.x1, titem.x2, titem.y1, titem.y2);
                    var tmpstn = faceSTNRunner.Predict(tmpimg);
                    var recogdata = faceRecogRunner.PredictTensor(tmpstn);
                    var tmpfls = recogdata.ToArray<float>();
                    if (hselist.Count > 0)
                    {
                        var tmprsp = await milBLL.Search(tmpfls, hselist, tscore);
                        if (tmprsp.IsSuccess())
                        {
                            if (tmprsp.Data.Count > 0)
                            {
                                var tmpitem = tmprsp.Data.First();
                                if (!addMemList.Exists(x => x.MemId == tmpitem.MemId))
                                {
                                    addMemList.Add(tmpitem);
                                    knowList.Add(tmpimg);
                                }
                            }
                            else
                            {
                                unknowList.Add(tmpimg);
                            }
                        }
                    }
                    else
                    {
                        unknowList.Add(tmpimg);
                    }
                }

                //触发熟人闯入事件
                if (addMemList.Count > 0)
                {
                    var tmemlist = await _provider.GetService<AiMemDAL>().SelectFaceMem(addMemList.Select(x => x.MemId).ToList());
                    foreach (var tmem in tmemlist)
                    {
                        int idx = addMemList.FindIndex(x => x.MemId == tmem.MemId);
                        if (idx >= 0)
                        {
                            var tmphhh = tmplist.Where(x => x.Id == addMemList[idx].HouseId).FirstOrDefault();
                            Dictionary<string, object> outputs = new Dictionary<string, object>();
                            outputs.Add("face_img", knowList[idx].ToBase64String(JpegFormat.Instance));
                            outputs.Add("face_name", tmem.MemInfo == null ? "佚名" : tmem.MemInfo.RealName);
                            if (tmphhh != null)
                            {
                                outputs.Add("face_house", tmphhh.HouseName);
                            }
                            else
                            {
                                outputs.Add("face_house", string.Empty);
                            }
                            outputs.Add("name_id", tmem.MemInfo == null ? string.Empty : tmem.MemInfo.Id.ToString());
                            await aiBusProxy.SendEvent(string.Empty, req.DeviceId, "KnwIn", outputs);
                        }
                    }

                }

                //触发陌生人闯入事件
                foreach (var unknow in unknowList)
                {
                    Dictionary<string, object> outputs = new Dictionary<string, object>();
                    outputs.Add("face_img", unknow.ToBase64String(JpegFormat.Instance));
                    await aiBusProxy.SendEvent(string.Empty, req.DeviceId, "UnkIn", outputs);
                }
            }

        }

        public override async Task Init(ITAServiceProvider provider)
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
