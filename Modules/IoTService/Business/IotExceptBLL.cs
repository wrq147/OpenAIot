using Common.IdGenerator;
using Common.Share;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using InfluxDB.Client;
using IoTService.DAL;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using ChannelUtility.Tsl;
using AuthService.Model;
using System.Linq.Expressions;
using MyAccess.DB.Builder.WhereToSql;

namespace IoTService.Business
{
    public class IotExceptBLL
    {
        private IotExceptDAL _exceptDAL;
        private ITAServiceProvider _provider;

        public IotExceptBLL(IotExceptDAL exceptDAL, ITAServiceProvider serviceProvider)
        {
            _exceptDAL = exceptDAL;
            _provider = serviceProvider;
        }

        public virtual async Task<BusResponse<int>> SaveExcept(List<MZ_PropExcept> points)
        {
            var rt = await _exceptDAL.Insert(points);
            return BusResponse<int>.Success(rt);
        }
        public virtual async Task<PageObject<ExceptProperty>> SelectExceptList(In_JVList query)
        {
            IotDeviceDAL deviceDAL = _provider.GetService<IotDeviceDAL>();
            IotProductDAL productDAL = _provider.GetService<IotProductDAL>();


            MZ_IotDevice device = await deviceDAL.Select(query.Id);
            var product = await productDAL.Select(device.ProductId);
            var model = TslModel.CreateFrom(product.ModelTSL);
            Dictionary<string, BaseProperty> hsdict = new Dictionary<string, BaseProperty>();
            foreach (BaseProperty bp in model.properties)
            {
                hsdict.Add(bp.code, bp);
            }

            BaseQueryParam bqp = new BaseQueryParam();
            if (query.pageNum != null && query.pageNum > 0)
            {
                bqp.pageNum = query.pageNum.Value;
                bqp.pageSize = query.pageSize.Value;
            }
            else
            {
                bqp.pageNum = 1;
                query.pageNum = 100;
            }


            Expression<Func<MZ_PropExcept, bool>> expression = x => x.DtuId == device.DeviceId;
            if (!string.IsNullOrEmpty(query.Code))
            {
                expression = expression.And(x => x.PropCode == query.Code);
            }

            DateTime? queryStart = query.BeginTime;
            DateTime? queryEnd = query.EndTime;
            if (queryStart != null && queryEnd != null)
            {
                expression = expression.And(x => x.CreatedOn >= MyAccess.Core.TypeConvert.Time2Unix(queryStart.Value));
                expression = expression.And(x => x.CreatedOn <= MyAccess.Core.TypeConvert.Time2Unix(queryEnd.Value));
            }

            var rt = new PageObject<ExceptProperty>();
            var tmppage = await _exceptDAL.SelectPage(expression, bqp, "CreatedOn desc");
            rt.List = new List<ExceptProperty>();
            for (int j = 0; j < tmppage.List.Count; j++)
            {
                try
                {
                    var tmpitem = tmppage.List[j];
                    //j是数据
                    DateTime? time = MyAccess.Core.TypeConvert.Unix2Time(tmpitem.CreatedOn.Value);
                    BaseProperty pp;
                    if (!hsdict.TryGetValue(tmpitem.PropCode, out pp))
                    {
                        continue;
                    }
                    switch (pp.option.type)
                    {
                        case "int":
                            {
                                var tmpintop = ((IntOption)pp.option);
                                rt.List.Add(new ExceptProperty()
                                {
                                    Name = pp.name,
                                    Code = pp.code,
                                    Value = Convert.ToInt32(tmpitem.ExceptValue),
                                    ExceptType = tmpitem.ExceptType,
                                    Unit = tmpintop.unit,
                                    UpdatedOn = time,
                                    OptionType = pp.option.type,
                                    Description = pp.description
                                });
                            }
                            break;
                        case "float":
                            {
                                var tmpfloatop = ((FloatOption)pp.option);
                                rt.List.Add(new ExceptProperty()
                                {
                                    Name = pp.name,
                                    Code = pp.code,
                                    Value = Convert.ToDouble(tmpitem.ExceptValue),
                                    ExceptType = tmpitem.ExceptType,
                                    Unit = tmpfloatop.unit,
                                    UpdatedOn = time,
                                    OptionType = pp.option.type,
                                    Description = pp.description
                                });
                            }
                            break;
                    }
                }
                catch { }

            }
            rt.Total = rt.List.Count;
            return rt;
        }
    }
}
