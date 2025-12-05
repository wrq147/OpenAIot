using Common.Share;
using MESService.DAL;
using MESService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class BatchDevHisBLL
    {
        private ITAServiceProvider _provider;
        private BatchDevHisDAL _batchDevHistDAL;
        public BatchDevHisBLL(ITAServiceProvider provider, BatchDevHisDAL batchDevHistDAL)
        {
            _provider = provider;
            _batchDevHistDAL = batchDevHistDAL;
        }
        public virtual async Task<PageObject<MZ_BatchDevHis>> SelectByPage(In_BatchDevHisList query, IUserInfo user)
        {
            return await _batchDevHistDAL.SelectByPage(query, user);
        }
        public virtual async Task<BusResponse<int>> Insert(In_BatchDevHisData data, IUserInfo user)
        {
            List<MZ_BatchDevHis> items = new List<MZ_BatchDevHis>();
            foreach (var item in data.Items)
            {
                MZ_BatchDevHis newhis = new MZ_BatchDevHis();
                newhis.OperId = data.OperId;
                newhis.CreatedOn = DateTime.Now;
                newhis.BatchNo = data.BatchNo;
                newhis.PropName = item.PropName;
                newhis.PropType = item.PropType;
                switch (item.PropType)
                {
                    case "int":
                    case "float":
                    case "date":
                        newhis.PropVal = Convert.ToDouble(item.PropVal);
                        break;
                    case "string":
                    case "enum":
                    case "boolean":
                        newhis.PropStrVal = Convert.ToString(item.PropVal);
                        break;
                }
                newhis.PropCode = item.PropCode;
                items.Add(newhis);
            }
            var rs = await _batchDevHistDAL.Insert(items);
            return BusResponse<int>.Success(rs);
        }
    }
}
