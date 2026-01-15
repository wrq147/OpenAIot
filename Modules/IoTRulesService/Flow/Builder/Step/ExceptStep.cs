
using Common;
using IoTRulesService.Flow.Node;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// AI数据异常检测
    /// 目前只提供三种：峰值、拐点
    /// </summary>
    public class ExceptStep : RuleflowStep
    {
        public ExceptProps props { get; set; }

        public override async Task Run(RuleExecutionContext context)
        {
            var countStep = context.FindStep(props.CountId);
            CountStep ccstep = countStep as CountStep;
            if (ccstep == null)
            {
                if (context.IsDebug)
                {
                    await context.Print("未选择检测的节拍器");
                }
                return;
            }
            if (!ccstep.IsActive)
            {
                return;
            }
            var stepdata = ccstep.GetData();
            if (stepdata.Count < 15)
            {
                await context.Print("节拍器聚合数据少于15条,无法检测");
                return;
            }
            if (context.IsDebug)
            {
                await context.Print("开始检测数据：" + Newtonsoft.Json.JsonConvert.SerializeObject(stepdata));
            }
            var tsl = await context.GetTsl(context.Source.ProductId);
            if (tsl == null)
            {
                await context.Print("物模型不存在");
                return;
            }
            var pp = tsl.Model.properties.Where(x => x.code == ccstep.FieldName).FirstOrDefault();
            if (pp == null || (pp.option.type != "int" && pp.option.type != "float"))
            {
                await context.Print("指定的节拍数据格式错误");
                return;
            }
            this.IsActive = false;
            List<StreamData> prelist = null;
            try
            {
                prelist = Prediction(stepdata, context);
            }
            catch (Exception ex)
            {
                await context.Print("异常检测出现未知错误" + ex.Message);
                return;
            }


            //去除重复检测到的异常数据
            CacheHelper cache = context.Provider.GetService<CacheHelper>();
            string streamKey = "RuleExceptList:" + context.Source.DeviceId + ":" + context.RuleId + ":" + this.Index;
            var oldExceptTimes = cache.GetCache<List<long>>(streamKey);
            if (oldExceptTimes != null)
            {
                for (int i = prelist.Count - 1; i >= 0; i--)
                {
                    if (oldExceptTimes.Contains(prelist[i].Time))
                    {
                        prelist.RemoveAt(i);
                    }
                }
            }
            else
            {
                oldExceptTimes = new List<long>();
            }

            if (prelist.Count > 0)
            {

                if (context.IsDebug)
                {
                    await context.Print("检测到异常数据：" + Newtonsoft.Json.JsonConvert.SerializeObject(prelist));
                }
                this.IsActive = true;

                try
                {
                    //储存用于去重的当前异常
                    int delcount = ccstep.Count - prelist.Count;
                    if (delcount > 0)
                    {
                        if (delcount >= oldExceptTimes.Count)
                        {
                            oldExceptTimes.Clear();
                        }
                        else
                        {
                            oldExceptTimes.RemoveRange(0, delcount);
                        }
                    }
                    foreach (var preitem in prelist)
                    {
                        oldExceptTimes.Add(preitem.Time);
                    }
                    cache.SetCache(streamKey, oldExceptTimes, DateTime.Now.AddHours(1));


                    //储存异常到数据库
                    List<MZ_PropExcept> points = new List<MZ_PropExcept>();
                    foreach (var preitem in prelist)
                    {
                        if (preitem.Data.Values.Count > 0)
                        {
                            object exceptval = preitem.Data.Values.ElementAt(0);
                            points.Add(new MZ_PropExcept()
                            {
                                CreatedOn = preitem.Time,
                                PropCode = preitem.Data.Keys.ElementAt(0),
                                ExceptValue = (exceptval ?? "").ToString(),
                                ExceptType = this.props.ExceptType,
                                DtuId = context.Source.DeviceId
                            });
                        }
                    }
                    if (points.Count > 0)
                    {
                        await context.Provider.GetService<IotExceptBLL>().SaveExcept(points);
                    }
                }
                catch
                {
                    await context.Print("异常存储失败");
                }


                await context.ExcuteNext(RuleResult.Next());
                return;
            }
            else
            {
                if (context.IsDebug)
                {
                    await context.Print("未检测到异常");
                }
                cache.SetCache(streamKey, oldExceptTimes, DateTime.Now.AddHours(1));
            }
        }

        public List<StreamData> Prediction(List<StreamData> inputList, RuleExecutionContext context)
        {
            var tmplist = inputList.OrderBy(x => x.Time).ToList();
            if (props.ExceptType == "spike")
            {
                return TimeSeriesDetector.DetectSpike(tmplist, this.props.windowSize);
            }
            else if (props.ExceptType == "change")
            {
                return TimeSeriesDetector.DetectInflectionPoints(tmplist);
            }
            else if (props.ExceptType == "range")
            {
                List<StreamData> ret = new List<StreamData>();
                foreach (var ipt in inputList)
                {
                    if (ipt.Data.Values.Count > 0)
                    {
                        double v = TAConverter.Cast<double>(ipt.Data.Values.ElementAt(0));
                        if (v > props.Max || v < props.Min)
                        {
                            ret.Add(ipt);
                        }
                    }
                }
                return ret;
            }
            return new List<StreamData>();
        }
    }


    public static class TimeSeriesDetector
    {
        #region 峰值检测
        /// <summary>
        /// 滑动窗口法检测峰值（极大值/极小值）
        /// </summary>
        /// <param name="data">时序数据</param>
        /// <param name="halfWindow">半邻域窗口大小</param>
        /// <returns>峰值列表</returns>
        public static List<StreamData> DetectSpike(List<StreamData> data, int halfWindow = 6)
        {
            int windowSize = halfWindow * 2 + 1;
            var peaks = new List<StreamData>();
            if (data == null || data.Count < windowSize)
            {
                return peaks;
            }

            int dataCount = data.Count;

            for (int i = halfWindow; i < dataCount - halfWindow; i++)
            {
                var currentEle = data[i];
                double currentValue = Convert.ToDouble(currentEle.Data.Values.ElementAt(0));
                bool isMax = true;
                bool isMin = true;

                // 检查窗口内的所有点
                for (int j = i - halfWindow; j <= i + halfWindow; j++)
                {
                    var compareEle = data[j];
                    double compareVal = Convert.ToDouble(compareEle.Data.Values.ElementAt(0));
                    if (j == i) continue; // 跳过当前点
                    if (compareVal >= currentValue) isMax = false;
                    if (compareVal <= currentValue) isMin = false;
                }

                // 标记峰值
                if (isMax)
                {
                    peaks.Add(currentEle);
                }
                else if (isMin)
                {
                    peaks.Add(currentEle);
                }
            }

            return peaks;
        }
        #endregion

        #region 拐点检测
        /// <summary>
        /// 差分法检测拐点（趋势反转点）
        /// </summary>
        /// <param name="data">时序数据</param>
        /// <param name="halfWindow">平滑窗口半宽（建议小于数据长度的1/2）</param>
        /// <returns>拐点列表</returns>
        public static List<StreamData> DetectInflectionPoints(List<StreamData> data, int halfWindow = 6)
        {
            var inflections = new List<StreamData>();
            // 基础校验：数据量至少需要3个点才能检测拐点
            if (data == null || data.Count < 3)
            {
                return inflections;
            }

            // 校验窗口大小：避免窗口过大导致平滑失效
            halfWindow = Math.Max(1, Math.Min(halfWindow, data.Count / 2 - 1));

            // 第一步：对数据进行平滑
            List<FeaturePoint> smoothedData = SmoothData(data, halfWindow);

            // 第二步：计算一阶差分（相邻点的差值）
            List<double> diffs = new List<double>();
            for (int i = 1; i < smoothedData.Count; i++)
            {
                diffs.Add(smoothedData[i].Value - smoothedData[i - 1].Value);
            }

            // 第三步：检测差分符号变化的点（趋势反转）
            for (int i = 1; i < diffs.Count; i++)
            {
                double prevDiff = diffs[i - 1];
                double currDiff = diffs[i];

                // 计算前后差分的符号（0视为无符号）
                int prevSign = Math.Sign(prevDiff);
                int currSign = Math.Sign(currDiff);

                // 检测有效符号变化：
                // 1. 正→负 或 负→正（严格反转）
                // 2. 零→正/负 或 正/负→零（平台后反转）
                bool isSignChanged = (prevSign != currSign) && (prevSign != 0 || currSign != 0);

                if (isSignChanged)
                {
                    inflections.Add(data[i]);
                }
            }

            return inflections;
        }

        /// <summary>
        /// 移动平均平滑数据（对称窗口，边界安全）
        /// </summary>
        /// <param name="data">原始时序数据</param>
        /// <param name="halfWindow">平滑窗口半宽</param>
        /// <returns>平滑后的特征点列表</returns>
        private static List<FeaturePoint> SmoothData(List<StreamData> data, int halfWindow)
        {
            var smoothed = new List<FeaturePoint>();
            for (int i = 0; i < data.Count; i++)
            {
                // 计算窗口的起止索引（保证不越界）
                int start = Math.Max(0, i - halfWindow);
                int end = Math.Min(data.Count - 1, i + halfWindow);

                double sum = 0;
                int count = 0;

                for (int j = start; j <= end; j++)
                {
                    var currentEle = data[j];
                    var currentValue = Convert.ToDouble(currentEle.Data.Values.ElementAt(0));
                    sum += currentValue;
                    count++;
                }

                // 避免除零（理论上count至少为1，因start<=end）
                double avgValue = count > 0 ? sum / count : 0;

                smoothed.Add(new FeaturePoint
                {
                    Index = i,
                    Value = avgValue
                });
            }
            return smoothed;
        }
        #endregion
    }
    public struct FeaturePoint
    {
        /// <summary>
        /// 数据点索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 数值
        /// </summary>
        public double Value { get; set; }
    }
}
