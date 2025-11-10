using Common;
using IoTService.Models;
using MyAccess.DB;
using MyAccess.DB.Builder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTService.DAL
{
    public class IotCardDAL : BaseRepository<MZ_IotCard>
    {
        public virtual async Task<List<MZ_IotCard>> QueryCards(string number)
        {
            return (await new SqlBuilder(help).Append("select c.* from mz_iot_card c inner join mz_iot_device d on c.UsingDevice=d.Id where d.DeviceNumber=").AppendParam(number).DoAsync<DoQuerySql<MZ_IotCard>>()).ToList();
        }
        public virtual async Task<List<MZ_IotCard>> QueryCardList(string[] numbers)
        {
            return (await new SqlBuilder(help).Append("select c.* from mz_iot_card c inner join mz_iot_device d on c.UsingDevice=d.Id where d.DeviceNumber in (").AppendParam(numbers).Append(")").DoAsync<DoQuerySql<MZ_IotCard>>()).ToList();
        }
        public virtual async Task<int> AddExpireDate(string id,int month)
        {
            return (await new SqlBuilder(help).Append("update mz_iot_card set ExpirationDate=DATE_ADD(IFNULL(ExpirationDate,NOW()),INTERVAL 1 MONTH) where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> UpdateCardList(List<MZ_IotCard> cards)
        {
            var sqb = new SqlBuilder(help);
            UpdateBuilder<MZ_IotCard> tmpsqb = null;
            foreach (var card in cards)
            {
                if (tmpsqb == null)
                {
                    tmpsqb = sqb.Update(card);
                }
                else
                {
                    tmpsqb = tmpsqb.Update(card);
                }
            }
            if (tmpsqb == null)
            {
                return 0;
            }
            return await tmpsqb.DoAsync();
        }
    }
}
