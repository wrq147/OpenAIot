using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotConfigBLL
    {
        private ITAServiceProvider _provider;
        public IotConfigBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public virtual async Task<BusResponse<int>> UpdateConfig(MZ_IotConfig data)
        {
            var configDAL = _provider.GetService<IotConfigDAL>();
            try
            {
                if (await configDAL.Some(x => x.OrgId == data.OrgId))
                {
                    return BusResponse<int>.Success(await configDAL.Update(data));
                }
                else
                {
                    if (data.EnableAutoAdd == null)
                    {
                        data.EnableAutoAdd = false;
                    }
                    return BusResponse<int>.Success(await configDAL.Insert(data));
                }

            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }

        public virtual async Task<MZ_IotConfig> Info(long id)
        {
            var configDAL = _provider.GetService<IotConfigDAL>();
            return await configDAL.Select(id);
        }
    }
}
