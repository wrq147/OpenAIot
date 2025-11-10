using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Third.Api.SimBoss
{
    public class DeviceRechargeRequest : ThreeIdCombineModel, SimbossRequest
    {

        public String GetUri()
        {
            return UriConstants.URI_DEVICE_RECHARGE;
        }

        public SortedDictionary<String, String> GetParam()
        {
            SortedDictionary<String, String> map = base.GetBaseParam();
            if (RatePlanId == 0)
            {
                throw new SimbossException("param ratePlanId is required");
            }
            map.Add("ratePlanId", RatePlanId.ToString());
            Month = Month == null ? 1 : Month;
            map.Add("month", Month.ToString());
            if (ExternalOrder != null)
            {
                map.Add("externalOrder", ExternalOrder);
            }
            return map;
        }

        public Int32? RatePlanId
        {
            get; set;
        }

        public Int32? Month
        {
            get; set;
        }

        public String ExternalOrder
        {
            get; set;
        }

        public String AppKey
        {
            get; set;
        }

    }
}
