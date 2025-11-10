using AuthService;
using Common.IdGenerator;
using Common.Share;
using Quartz.Impl.AdoJobStore.Common;
using StorageService.DAL;
using StorageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace StorageService.Business
{
    public class StReportBLL
    {
        private ITAServiceProvider _provider;
        private OperatorHelper _operatorHelper;
        private SnowflakeHelper _snowflake;
        private StockPileDAL _pileDAL;
        private StockRecordDAL _stockRecordDAL;
        public StReportBLL(ITAServiceProvider provider, OperatorHelper operatorHelper, SnowflakeHelper snowflake, StockPileDAL pileDAL, StockRecordDAL stockRecordDAL)
        {
            _provider = provider;
            _operatorHelper = operatorHelper;
            _snowflake = snowflake;
            _pileDAL = pileDAL;
            _stockRecordDAL = stockRecordDAL;
        }


        public virtual async Task<Out_StockStatistics> StockInfo()
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider, "/StorageService/House/List");
            Out_StockStatistics stockStatistics = await _pileDAL.SelectStockStatistics(user, scope);

            if (stockStatistics == null)
            {
                stockStatistics = new Out_StockStatistics();
            }
            stockStatistics.TotalCount = stockStatistics.DevCount + stockStatistics.PartsCount;

            var tmpToday = await _pileDAL.SelectTodayStockStatistics(user, scope);
            if (tmpToday == null)
            {
                tmpToday = new Out_StockStatistics();
            }
            stockStatistics.TodayPartsInCount = tmpToday.TodayPartsInCount;
            stockStatistics.TodayDevInCount = tmpToday.TodayDevInCount;
            stockStatistics.TodayInCount = tmpToday.TodayPartsInCount + tmpToday.TodayDevInCount;

            stockStatistics.TodayPartsOutCount = tmpToday.TodayPartsOutCount;
            stockStatistics.TodayDevOutCount = tmpToday.TodayDevOutCount;
            stockStatistics.TodayOutCount = tmpToday.TodayPartsOutCount + tmpToday.TodayDevOutCount;
            return stockStatistics;
        }
        public virtual async Task<PageObject<Out_StockRecord>> SelectDetailByPage(In_DetailRecordPage query)
        {
            if (query.Day != null)
            {
                if (query.Day == 0)
                {
                    query.beginTime = DateTime.Today;
                }
                else if (query.Day == 1)
                {
                    query.beginTime = DateTime.Today.AddDays(-1);
                    query.endTime = DateTime.Today;
                }
                else
                {
                    query.beginTime = DateTime.Today.AddDays(-query.Day.Value);
                }
            }

            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider, "/StorageService/House/List");
            return await _stockRecordDAL.SelectDetailByPage(query, user, scope);
        }
    }
}
