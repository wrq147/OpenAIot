namespace HttpChannel.AEP
{
    public class Aep_product_management
    {
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        public static string QueryProduct(string appKey, string appSecret, string productId)
        {
            string path = "/aep_product_management/product";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);

            string version = "20181031202055";

            string application = appKey;
            string key = appSecret;


            string response = AepSdkCore.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数searchValue: 类型String, 参数可以为空
        //  描述:产品id或者产品名称
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数
        public static string QueryProductList(string appKey, string appSecret, string searchValue = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_product_management/products";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("searchValue", searchValue);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20190507004824";

            string application = appKey;
            string key = appSecret;


            string response = AepSdkCore.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        public static string DeleteProduct(string appKey, string appSecret, string MasterKey, string productId)
        {
            string path = "/aep_product_management/product";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);

            string version = "20181031202029";

            string application = appKey;
            string key = appSecret;


            string response = AepSdkCore.SendAepHttpRequest(path, headers, param, null, version, application, key, "DELETE");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string CreateProduct(string appKey, string appSecret, string body)
        {
            string path = "/aep_product_management/product";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20220924042921";

            string application = appKey;
            string key = appSecret;


            string response = AepSdkCore.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string UpdateProduct(string appKey, string appSecret, string body)
        {
            string path = "/aep_product_management/product";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20220924043504";

            string application = appKey;
            string key = appSecret;


            string response = AepSdkCore.SendAepHttpRequest(path, headers, param, body, version, application, key, "PUT");
            if (response != null)
                return response;
            return null;
        }
    }
}
