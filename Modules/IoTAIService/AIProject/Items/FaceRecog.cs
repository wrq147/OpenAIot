using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
using IoTAIService.Business;
using IoTAIService.DAL;
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
    public class FaceRecog : IInfer
    {
        private ITAServiceProvider _provider;

        public async Task Execute(AIDetectRequestMeesage req, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
            var tboxlist = boxes.Where(x => x.label == "人脸").ToList();
            var facenum = tboxlist.Count;
            var aiCache = _provider.GetService<AICache>();
            var videoData = aiCache.GetVideoCache(req.DeviceId);
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

            #region 跟踪人脸框
            var tracker = videoData.GetItem<ByteTrack>("face_track");
            if (tracker == null)
            {
                tracker = new ByteTrack(trackThresh: 0.5f, trackLowThresh: 0.1f, matchThresh: 0.8f);
            }
            (var tracklist, var addlist, var rmlist) = tracker.Update(tboxlist);
            #endregion

            if (facenum > 0 && addlist.Count > 0)
            {
                var nowtime = DateTime.Now;
                var tmplist = await _provider.GetService<AiHouseDAL>().SelectList(x => x.OrgId == config.OrgId);
                List<string> hselist = tmplist.Select(x => x.Id).ToList();

                //处理人脸识别事件
                float tscore = config.GetFloat("facescore", 0.8f);
                var faceSTNRunner = _provider.GetService<FaceSTNRunner>();
                var faceRecogRunner = _provider.GetService<FaceRecogRunner>();
                var milBLL = _provider.GetService<MilvusBLL>();


                List<long> addMemList = new List<long>();
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
                                long tmpid = tmprsp.Data.First();
                                if (!addMemList.Contains(tmpid))
                                {
                                    addMemList.Add(tmpid);
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
                    var tmemlist = await _provider.GetService<AiMemDAL>().SelectFaceMem(addMemList);
                    foreach (var tmem in tmemlist)
                    {
                        int idx = addMemList.IndexOf(tmem.MemId.Value);
                        if (idx >= 0)
                        {
                            Dictionary<string, object> outputs = new Dictionary<string, object>();
                            outputs.Add("face_img", knowList[idx].ToBase64String(JpegFormat.Instance));
                            outputs.Add("face_name", tmem.MemInfo == null ? "佚名" : tmem.MemInfo.RealName);
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


                var tmpkeyTime = videoData.GetDateTime("LastKeyTime", DateTime.Now.AddHours(-1));
                if ((DateTime.Now - tmpkeyTime).TotalSeconds > 60)
                {
                    var fileHelper = _provider.GetService<FileHelper>();
                    var turl = await fileHelper.UploadRgb24File(image);
                    if (!string.IsNullOrEmpty(turl))
                    {
                        videoData.SetDateTime("LastKeyTime", DateTime.Now);
                        await aiBusProxy.SendMediaKey(req.DeviceId, req.VideoKey, "人员闯入", turl);
                    }
                }
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
