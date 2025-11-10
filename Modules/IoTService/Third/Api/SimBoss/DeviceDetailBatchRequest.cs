using System;
using System.Collections.Generic;

namespace IoTService.Third.Api.SimBoss
{
    public class DeviceDetailBatchRequest : ThreeIdsCombineModel, SimbossRequest
    {

        public String GetUri()
        {
            return UriConstants.URI_DEVICE_DETAIL_BATCH;
        }

        public SortedDictionary<String, String> GetParam()
        {
            SortedDictionary<String, String> map = base.GetBaseParam();
            return map;
        }
    }
}
