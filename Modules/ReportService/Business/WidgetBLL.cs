using AuthService;
using Common.IdGenerator;
using Common.Share;
using ReportService.DAL;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace ReportService.Business
{
    public class WidgetBLL
    {
        private WidgetDAL _widgetDAL;
        private SnowflakeHelper _snowflake;
        private ITAServiceProvider _provider;
        public WidgetBLL(WidgetDAL widgetDAL, SnowflakeHelper snowflake, ITAServiceProvider serviceProvider)
        {
            _widgetDAL = widgetDAL;
            _snowflake = snowflake;
            _provider = serviceProvider;
        }

        public virtual async Task<MZ_ReportCom> GetWidget(int id)
        {
            return await _widgetDAL.Select(id);
        }
        public virtual async Task<BusResponse<int>> AddWidget(MZ_ReportCom data)
        {
            try
            {
                var user = _provider.GetUser();
                if (user.OrgId <= 0)
                {
                    return BusResponse<int>.Error(112, "非企业用户无法添加组件");
                }
                data.Id = _snowflake.NextId().ToString();
                data.OrgId = user.OrgId;
                data.SetCreateBy(user);
                return BusResponse<int>.Success(await _widgetDAL.Insert(data));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> UpdateWidget(MZ_ReportCom data)
        {
            try
            {
                var user = _provider.GetUser();
                data.SetUpdateBy(user);
                return BusResponse<int>.Success(await _widgetDAL.Update(data, x => x.Id == data.Id && x.OrgId == user.OrgId));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var user = _provider.GetUser();
            return BusResponse<int>.Success(await _widgetDAL.Delete(x => x.Id == id && x.OrgId == user.OrgId));
        }
    }
}
