using System;
using System.Collections.Generic;

namespace IoTService.Third.Api.SimBoss
{
    public class CardPoolListRequest : SimbossRequest
    {

        public String GetUri()
        {
            return UriConstants.URI_CARD_POOL_LIST;
        }

        public SortedDictionary<String, String> GetParam()
        {
            SortedDictionary<String, String> map = new SortedDictionary<String, String>();
            return map;
        }
    }
}
