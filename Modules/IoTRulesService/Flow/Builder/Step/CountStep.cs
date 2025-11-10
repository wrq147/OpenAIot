using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// 聚合节点（聚合设备数据用）
    /// </summary>
    public class CountStep : RuleflowStep
    {
        /// <summary>
        /// 计数方式：0为次数、1为时间秒计数
        /// </summary>
        public int Way { get; set; }
        /// <summary>
        /// 聚合的字段
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 计数值
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 保留计数（事件触发后保留的数据量，默认为0）
        /// </summary>
        public int CountLen { get; set; }

        private List<StreamData> _activeData;
        public List<StreamData> GetData()
        {
            return _activeData;
        }
        public override async Task Run(RuleExecutionContext context)
        {
            this.IsActive = false;
            List<StreamData> needlist = new List<StreamData>();
            foreach (var strea in context.Data)
            {
                if (strea.Data.TryGetValue(FieldName, out var data))
                {
                    needlist.Add(new StreamData()
                    {
                        Time = strea.Time,
                        Data = new Dictionary<string, object>()
                        {
                            {FieldName, data}
                        }
                    });
                }
            }
            if (needlist.Count == 0)
            {
                await context.ExcuteNext(RuleResult.Next());
                return;
            }

            CacheHelper cache = context.Provider.GetService<CacheHelper>();
            string streamKey = "RuleCount" + Way + ":" + context.Source.DeviceId + ":" + context.RuleId + ":" + this.Index;

            var tlist = cache.GetCache<List<StreamData>>(streamKey);
            if (tlist == null)
            {
                tlist = new List<StreamData>();
            }
            tlist.AddRange(needlist);
            //判断是否激活下一个节点
            if (Way == 0)
            {
                if (context.IsDebug)
                {
                    await context.Print("当前计数：" + tlist.Count);
                }
                if (tlist.Count >= Count)
                {
                    _activeData = new List<StreamData>();
                    _activeData.AddRange(tlist);
                    if (CountLen <= 0)
                    {
                        cache.RemoveCache(streamKey);
                    }
                    else
                    {
                        if (CountLen < tlist.Count)
                        {
                            tlist.RemoveRange(0, tlist.Count - CountLen);
                        }
                    }
                    this.IsActive = true;
                }
            }
            else
            {
                DateTime now = DateTime.Now;
                long timeDelta = (MyAccess.Core.TypeConvert.Time2Unix(now) - tlist[0].Time) / 1000;
                if (context.IsDebug)
                {
                    await context.Print("当前计数时间差：" + timeDelta + "秒");
                }

                if (timeDelta > Count)
                {
                    _activeData = new List<StreamData>();
                    _activeData.AddRange(tlist);
                    if (CountLen <= 0)
                    {
                        cache.RemoveCache(streamKey);
                    }
                    else
                    {
                        int endidx = 0;
                        long oldtime = MyAccess.Core.TypeConvert.Time2Unix(now.AddSeconds(-CountLen));
                        for (int i = 0; i < tlist.Count; i++)
                        {
                            StreamData t = tlist[i];
                            if (t.Time >= oldtime)
                            {
                                endidx = i;
                                break;
                            }
                        }

                        if (endidx > 0)
                        {
                            tlist.RemoveRange(0, endidx);
                        }
                    }
                    this.IsActive = true;
                }
            }

            if (!this.IsActive)
            {
                cache.SetCache(streamKey, tlist);
            }
            await context.ExcuteNext(RuleResult.Next());
        }
    }

}
