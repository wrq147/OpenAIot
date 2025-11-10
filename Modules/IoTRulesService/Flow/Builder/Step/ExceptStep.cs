
using Common;
using IoTRulesService.Flow.Node;
using IoTService;
using IoTService.Business;
using IoTService.DAL;
using IoTService.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
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
    /// 目前只提供三种：峰值、拐点、SRCNN
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


                    //储存异常到时序数据库
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
                    await context.Provider.GetService<IotExceptBLL>().SaveExcept(points);
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
        private CacheModel CreateSrCNNModel(RuleExecutionContext context, List<ExceptInputItem> inputs)
        {
            CacheHelper cache = context.Provider.GetService<CacheHelper>();
            string streamKey = "SrCNNModel:" + context.Source.DeviceId + ":" + context.RuleId + ":" + this.Index;
            var model = cache.GetCache<CacheModel>(streamKey);
            if (model == null)
            {
                model = new CacheModel();
                model.Context = new MLContext();
                var dataView = model.Context.Data.LoadFromEnumerable(inputs);
                model.Trainer = model.Context.Transforms.DetectAnomalyBySrCnn(nameof(ExceptOutputItem.Prediction), nameof(ExceptInputItem.Value), inputs.Count, 5, 5, 3, 8, 0.35);
                model.Model = model.Trainer.Fit(dataView);
            }
            else
            {
                var dataView = model.Context.Data.LoadFromEnumerable(inputs);
                model.Model = model.Trainer.Fit(dataView);
            }
            cache.SetCache(streamKey, model, DateTime.Now.AddHours(1));
            return model;
        }
        public List<StreamData> Prediction(List<StreamData> inputList, RuleExecutionContext context)
        {
            var tmplist = inputList.OrderBy(x => x.Time).ToList();
            if (props.ExceptType == "spike")
            {
                var mlContext = new MLContext();
                var input = GenerateInput(tmplist);
                if (input.Count == 0)
                {
                    return new List<StreamData>();
                }

                var dataView = mlContext.Data.LoadFromEnumerable(input);
                var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(nameof(ExceptOutputItem.Prediction), nameof(ExceptInputItem.Value), props.Confidence, input.Count);
                var empty = mlContext.Data.LoadFromEnumerable(new List<ExceptInputItem>());
                ITransformer iidSpikeTransform = iidSpikeEstimator.Fit(empty);
                IDataView transformedData = iidSpikeTransform.Transform(dataView);
                var predictions = mlContext.Data.CreateEnumerable<ExceptOutputItem>(transformedData, false);

                int i = 0;
                int endidx = inputList.Count - 2;
                List<StreamData> ret = new List<StreamData>();
                foreach (var prediction in predictions)
                {
                    if (i > 2 && i < endidx)
                    {
                        if (prediction.Prediction[0] == 1)
                        {
                            ret.Add(inputList[i]);
                        }
                    }
                    ++i;
                }
                return ret;
            }
            else if (props.ExceptType == "change")
            {
                var mlContext = new MLContext();
                var input = GenerateInput(tmplist);
                if (input.Count == 0)
                {
                    return new List<StreamData>();
                }
                var dataView = mlContext.Data.LoadFromEnumerable(input);
                var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(nameof(ExceptOutputItem.Prediction), nameof(ExceptInputItem.Value), props.Confidence, input.Count);
                var empty = mlContext.Data.LoadFromEnumerable(new List<ExceptInputItem>());
                ITransformer iidChangeTransform = iidChangePointEstimator.Fit(empty);
                IDataView transformedData = iidChangeTransform.Transform(dataView);
                var predictions = mlContext.Data.CreateEnumerable<ExceptOutputItem>(transformedData, false);
                int i = 0;
                int endidx = inputList.Count - 2;
                List<StreamData> ret = new List<StreamData>();
                foreach (var prediction in predictions)
                {
                    if (i > 2 && i < endidx)
                    {
                        if (prediction.Prediction[0] == 1)
                        {
                            ret.Add(inputList[i]);
                        }
                    }

                    ++i;
                }
                return ret;
            }
            else if (props.ExceptType == "SRCNN")
            {
                var input = GenerateInput(tmplist);
                if (input.Count == 0)
                {
                    return new List<StreamData>();
                }
                //创建srcnn模型，边训练边预测
                var model = CreateSrCNNModel(context, input);
                var engine = model.CreateEngine();
                List<StreamData> ret = new List<StreamData>();
                for (int index = 0; index < input.Count; index++)
                {
                    var prers = engine.Predict(new ExceptInputItem(input[index].Value));
                    if (prers.Prediction[0] == 1 && inputList.Count > index)
                    {
                        ret.Add(inputList[index]);
                    }
                }

                return ret;
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
        /// <summary>
        /// 输入数据流转换成模型输入
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        private List<ExceptInputItem> GenerateInput(List<StreamData> inputList)
        {
            List<ExceptInputItem> items = new List<ExceptInputItem>();
            foreach (var ipt in inputList)
            {
                if (ipt.Data.Values.Count > 0)
                {
                    object val = ipt.Data.Values.ElementAt(0);
                    ExceptInputItem item = new ExceptInputItem(TAConverter.Cast<float>(val));
                    items.Add(item);
                }
            }
            return items;
        }

    }
    /// <summary>
    /// 检测的输入
    /// </summary>
    public class ExceptInputItem
    {
        public float Value;

        public ExceptInputItem(float value)
        {
            Value = value;
        }
    }
    /// <summary>
    /// 检测的输出
    /// </summary>
    public class ExceptOutputItem
    {
        [VectorType(3)]
        public double[] Prediction { get; set; }
    }
    public class CacheModel
    {
        public SrCnnAnomalyEstimator Trainer { get; set; }
        public ITransformer Model { get; set; }
        public MLContext Context { get; set; }
        public TimeSeriesPredictionEngine<ExceptInputItem, ExceptOutputItem> CreateEngine()
        {
            return Model.CreateTimeSeriesEngine<ExceptInputItem, ExceptOutputItem>(Context);
        }
    }
}
