using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;

namespace HttpChannel.AEP
{
    public class AepSdkCore
    {
        static long offset = 0;
        static long lastGetOffsetTime = 0;
        static readonly string baseUrl = "https://2000469552.api.ctwing.cn";
        static readonly string timeUrl = "https://2000469552.api.ctwing.cn/echo";


        /// <summary>
        /// 获取时间偏移量
        /// </summary>
        /// <returns>时间偏移量</returns>
        public static long GetTimeOffset()
        {
            long start = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            WebHeaderCollection head = null;

            if (timeUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;  //| SecurityProtocolType.Ssl3;//设置这个安全协议必须在创建请求之前！
            }

            string response = SendHttpRequest(timeUrl, null, "application/json; charset=UTF-8", "GET", null, 5, out head);
            long end = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            if (response != null)
            {
                long timeAg = Convert.ToInt64(head["x-ag-timestamp"]);
                return timeAg - (end + start) / 2;
            }

            return 0;
        }

        /// <summary>
        /// 发送api请求到aep
        /// </summary>
        /// <param name="path">api接口路径</param>
        /// <param name="headers">请求head</param>
        /// <param name="param">参数</param>
        /// <param name="body">body，如果为get等没有body请求，填null</param>
        /// <param name="version">api接口版本，在文档中查询</param>
        /// <param name="application">App Key</param>
        /// <param name="key">App Secret</param>
        /// <param name="method">请求的类型,GET、POST、PUT、DELETE</param>
        /// <returns></returns>
        public static string SendAepHttpRequest(string path, Dictionary<string, string> headers, Dictionary<string, string> param, string body, string version, string application, string key, string method)
        {
            WebHeaderCollection head;
            return SendAepHttpRequest(path, headers, param, body, version, application, key, method, out head);
        }


        /// <summary>
        /// 发送api请求到aep
        /// </summary>
        /// <param name="path">api接口路径</param>
        /// <param name="headers">请求head</param>
        /// <param name="param">参数</param>
        /// <param name="body">body，如果为get等没有body请求，填null</param>
        /// <param name="version">api接口版本，在文档中查询</param>
        /// <param name="application">App Key</param>
        /// <param name="key">App Secret</param>
        /// <param name="method">请求的类型,GET、POST、PUT、DELETE</param>
        /// <param name="head">请求结果的head出参</param>
        /// <returns></returns>
        public static string SendAepHttpRequest(string path, Dictionary<string, string> headers, Dictionary<string, string> param, string body, string version, string application, string key, string method, out WebHeaderCollection head)
        {

            string paramString = "";

            if (param != null)
            {
                foreach (KeyValuePair<string, string> kvp in param)
                {
                    paramString += kvp.Key + "=" + kvp.Value + "&";
                }
            }

            if (paramString.Length > 0)
            {
                paramString = paramString.Remove(paramString.Length - 1);
            }

            Console.WriteLine("paramString = " + paramString);



            Dictionary<string, string> paramTmp = new Dictionary<string, string>();
            if (headers != null)
                paramTmp = paramTmp.Concat(headers).ToDictionary(k => k.Key, v => v.Value);
            if (param != null)
                paramTmp = paramTmp.Concat(param).ToDictionary(k => k.Key, v => v.Value);

            long curentTime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            if (curentTime - lastGetOffsetTime > 300 * 1000) //300秒调用一次
            {
                offset = GetTimeOffset();
                lastGetOffsetTime = curentTime;
            }
            long timestamp = curentTime + offset;

            Dictionary<string, string> headersTmp = new Dictionary<string, string>();
            headersTmp.Add("application", application);
            headersTmp.Add("timestamp", "" + timestamp);
            headersTmp.Add("version", version);
            //headersTmp.Add("Content-Type", "application/json; charset=UTF-8");
            //headersTmp.Add("Date", dataString);
            headersTmp.Add("signature", Sign(paramTmp, timestamp, application, key, body));
            if (headers != null)
            {
                headersTmp = headersTmp.Concat(headers).ToDictionary(k => k.Key, v => v.Value);
            }

            string url = baseUrl + path;
            if (paramString.Length > 0)
            {
                url += "?" + paramString;
            }
            Console.WriteLine("url = " + url);

            //if (baseUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            //{
            //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;//设置这个安全协议必须在创建请求之前！
            //}


            string result = SendHttpRequest(url, headersTmp, "application/json; charset=UTF-8", method, body, 35, out head);
            return result;
        }



        /// <summary>
        /// 签名算法
        /// </summary>
        /// <param name="param">api接口参数</param>
        /// <param name="timestamp">时间戳，毫秒级</param>
        /// <param name="application">App Key</param>
        /// <param name="secret">App secret</param>
        /// <param name="body">body</param>
        /// <returns></returns>
        public static string Sign(Dictionary<string, string> param, long timestamp, string application, string secret, string body)
        {
            // 连接系统参数
            string temp = "application:" + application + "\n";
            temp += "timestamp:" + timestamp + "\n";

            // 连接请求参数
            if (param != null)
            {
                var dicNew = param.OrderBy(x => x.Key, new ComparerString()).ToDictionary(x => x.Key, y => y.Value);

                foreach (KeyValuePair<string, string> kvp in dicNew)
                {
                    temp += kvp.Key + ":" + (kvp.Value == null ? "" : kvp.Value) + "\n";
                }
            }


            // 得到需要签名的字符串
            if (body != null && body.Length > 0)
            {
                temp += body + "\n";
            }
            Console.WriteLine("Sign string: " + temp);

            // hmac-sha1编码
            var hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.UTF8.GetBytes(secret);
            byte[] dataBuffer = Encoding.UTF8.GetBytes(temp);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);


            // base64编码
            string encode = Convert.ToBase64String(hashBytes);

            return encode;
        }

        /// <summary>
        /// 处理http请求
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="headers">协议标头</param>
        /// <param name="contentType">请求的内容类型</param>
        /// <param name="method">请求的类型,GET、POST、PUT、DELETE</param>
        /// <param name="dataStream">请求的数据流</param>
        /// <param name="timeout">请求的超时时间（秒）</param>
        /// <returns>http POST成功后返回的数据，失败抛异常</returns>
        public static string SendHttpRequest(string url, Dictionary<string, string> headers, string contentType, string method, string dataStream, int timeout, out WebHeaderCollection head)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http链接
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;
            try
            {
                //设置最大链接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CertificateValidation);

                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;//设置这个安全协议必须在创建请求之前！
                }
                request = (HttpWebRequest)WebRequest.Create(url);

                //HttpWebRequest 相关属性
                request.Method = method;
                request.Timeout = timeout * 1000;
                request.ContentType = contentType;
                if (headers != null)
                {
                    //配置协议标头
                    foreach (KeyValuePair<string, string> kvp in headers)
                    {
                        request.Headers.Add(kvp.Key, kvp.Value);
                    }
                }

                byte[] data = null;
                if (dataStream != null)
                {
                    data = System.Text.Encoding.UTF8.GetBytes(dataStream);
                    request.ContentLength = data.Length;
                }

                if (data != null)
                {
                    //写入数据
                    reqStream = request.GetRequestStream();
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }

                head = null;
                //返回数据
                response = (HttpWebResponse)request.GetResponse();
                if (response != null)
                {
                    head = response.Headers;
                    Stream stream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                    string result = sr.ReadToEnd();
                    sr.Close();
                    //关闭连接和流
                    response.Close();
                    return result;
                }
                else
                {
                    head = null;
                    return String.Empty;
                }


            }
            //处理多线程模式下线程中止
            //catch (System.Threading.ThreadAbortException e)
            //{
            //    System.Threading.Thread.ResetAbort();
            //}
            catch (WebException e)
            {
                head = null;
                response = (HttpWebResponse)e.Response;
                if (response != null)
                {
                    head = response.Headers;
                }
                throw e;
            }
            catch (Exception e)
            {
                throw new HttpServiceException(e.ToString());
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        /* 忽略证书认证错误
         * .NET的SSL通信过程中，使用的证书可能存在各种问题
         * 此方法可以跳过服务器证书验证，完成正常通信。*/
        private static bool CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            // 认证正常，没有错误   
            return true;
        }
    }
    class HttpServiceException : Exception
    {
        public HttpServiceException(string msg) : base(msg)
        {

        }
    }

    public class ComparerString : IComparer<String>
    {
        public int Compare(String x, String y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}
