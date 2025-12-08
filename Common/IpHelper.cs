using Common.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common
{
    public class IpHelper
    {
        /// <summary>
        /// 获取可用的外网Ip
        /// </summary>
        /// <returns></returns>
        public static string GetAvaOutIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                string tmpip = ip.ToString();
                if (ip.AddressFamily == AddressFamily.InterNetwork && !InternalIp(tmpip))
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取真实ip
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetIpAddr(ITARequest request)
        {
            if (request == null)
            {
                return string.Empty;
            }
            string ip = request.Header["x-forwarded-for"];
            if (ip != null)
            {
                string[] tmparr = ip.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                ip = string.Empty;
                foreach (string s in tmparr)
                {
                    if (!InternalIp(s))
                    {
                        ip = s;
                        break;
                    }
                }
            }
            if (ip == null || ip.Length == 0 || "unknown".Equals(ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.Header["Proxy-Client-IP"];
            }
            if (ip == null || ip.Length == 0 || "unknown".Equals(ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.Header["X-Forwarded-For"];
            }
            if (ip == null || ip.Length == 0 || "unknown".Equals(ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.Header["WL-Proxy-Client-IP"];
            }
            if (ip == null || ip.Length == 0 || "unknown".Equals(ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.Header["X-Real-IP"];
            }

            if ("0:0:0:0:0:0:0:1".Equals(ip))
            {
                ip = null;
            }

            if (ip == null || ip.Length == 0 || "unknown".Equals(ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.ClientIP.MapToIPv4().ToString();
            }


            if (ip.StartsWith("::ffff:"))
            {
                ip = ip.Substring(7);
            }

            int ipidx = ip.LastIndexOf(":");
            if (ipidx > 0)
            {
                string newip = ip.Substring(0, ipidx);
                if (newip.Length < 16)
                {
                    ip = newip;
                }
            }
            return ip;
        }

        /// <summary>
        /// 将IPv4地址转换成字节
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] TextToNumericFormatV4(string ipAddress)
        {
            System.Net.IPAddress ip;
            if (System.Net.IPAddress.TryParse(ipAddress, out ip))
            {
                return ip.GetAddressBytes();
            }
            return null;
        }
        /// <summary>
        /// 判断是否为内网Ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool InternalIp(String ip)
        {
            byte[] addr = TextToNumericFormatV4(ip);
            return InternalIp(addr) || "127.0.0.1".Equals(ip);
        }

        private static bool InternalIp(byte[] addr)
        {
            if (addr == null || addr.Length < 2)
            {
                return true;
            }
            byte b0 = addr[0];
            byte b1 = addr[1];
            // 10.x.x.x/8
            const byte SECTION_1 = 0x0A;
            // 172.16.x.x/12
            const byte SECTION_2 = (byte)0xAC;
            const byte SECTION_3 = (byte)0x10;
            const byte SECTION_4 = (byte)0x1F;
            // 192.168.x.x/16
            const byte SECTION_5 = (byte)0xC0;
            const byte SECTION_6 = (byte)0xA8;
            switch (b0)
            {
                case SECTION_1:
                    return true;
                case SECTION_2:
                    if (b1 >= SECTION_3 && b1 <= SECTION_4)
                    {
                        return true;
                    }
                    return false;
                case SECTION_5:
                    switch (b1)
                    {
                        case SECTION_6:
                            return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        // IP地址查询
        public const String IP_URL = "http://whois.pconline.com.cn/ipJson.jsp";

        public static async Task<Dictionary<string, string>> GetAddressByIP(string ip)
        {
            string url = string.Concat(IP_URL, "?", "ip=" + ip + "&json=true");
            string rspStr = await HttpHelper.Instance.GetAsync(url, Encoding.GetEncoding("GBK"));
            if (string.IsNullOrEmpty(rspStr))
            {
                return null;
            }
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(rspStr, MyDefaultTextJsonConfig.DefaultOptions);
        }




        private QQWryIpSearch _search;
        private static object _lock = new object();
        private static IpHelper _instance;
        public static IpHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new IpHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private IpHelper()
        {
            var config = new QQWryOptions()
            {
                DbPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "qqwry.dat"
            };
            _search = new QQWryIpSearch(config);
        }
        public async Task<string> GetRealAddressByIP(string ip)
        {
            string address = string.Empty;
            // 内网不查询
            if (InternalIp(ip))
            {
                return "内网IP";
            }
            var ipll = GetIpLocation(ip);
            if (ipll != null && !string.IsNullOrEmpty(ipll.Country) && !string.IsNullOrEmpty(ipll.Province) && !string.IsNullOrEmpty(ipll.City))
            {
                return ipll.Country + "," + ipll.Province + "," + ipll.City;
            }
            try
            {
                var obj = await GetAddressByIP(ip);
                if (obj == null)
                {
                    return address;
                }
                string tmppro = obj["pro"];
                if (string.IsNullOrEmpty(tmppro))
                {
                    return "未知";
                }
                string tmpregion = obj["region"];
                if (string.IsNullOrEmpty(tmpregion))
                {
                    return string.Format("中国,{0},{1}", obj["pro"], obj["city"]);
                }
                return string.Format("中国,{0},{1},{2}", obj["pro"], obj["city"], obj["region"]);
            }
            catch { }
            return address;
        }

        public IpLocation GetIpLocation(string ip)
        {
            try
            {
                return _search.GetIpLocation(ip);
            }
            catch
            {
                return null;
            }
        }
    }


    /// <summary>
    /// QQWryIpSearch 请作为单例使用 数据库缓存在内存
    /// </summary>
    public class QQWryIpSearch : IIpSearch, IDisposable
    {
        private static readonly Encoding _encodingGb2312;
        private static string[] isparr = new string[] { "联通", "移动", "铁通", "电信", "长城", "聚友" };
        private static string[] isprovince = new string[] { "北京", "天津", "重庆", "上海", "河北", "山西", "辽宁", "吉林", "黑龙江", "江苏", "浙江", "安徽", "福建", "江西", "山东", "河南", "湖北", "湖南", "广东", "海南", "四川", "贵州", "云南", "陕西", "甘肃", "青海", "台湾", "内蒙古", "广西", "宁夏", "新疆", "西藏", "香港", "澳门" };
        private static string[] citydirectly = new string[] { "北京", "天津", "重庆", "上海" };
        private static string[] allct =
            new string[] {
"阿尔巴尼亚",
"阿尔及利亚",
"阿富汗",
"阿根廷",
"阿拉伯联合酋长国",
"阿鲁巴",
"阿曼",
"阿塞拜疆",
"阿森松岛",
"埃及",
"埃塞俄比亚",
"爱尔兰",
"爱沙尼亚",
"安道尔",
"安哥拉",
"安圭拉",
"安提瓜岛和巴布达",
"澳大利亚",
"奥地利",
"奥兰群岛",
"巴巴多斯岛",
"巴布亚新几内亚",
"巴哈马",
"巴基斯坦",
"巴拉圭",
"巴勒斯坦",
"巴林",
"巴拿马",
"巴西",
"白俄罗斯",
"百慕大",
"保加利亚",
"北马里亚纳群岛",
"贝宁",
"比利时",
"冰岛",
"波多黎各",
"波兰",
"玻利维亚",
"波斯尼亚和黑塞哥维那",
"博茨瓦纳",
"伯利兹",
"不丹",
"布基纳法索",
"布隆迪",
"布韦岛",
"朝鲜",
"丹麦",
"德国",
"东帝汶",
"多哥",
"多米尼加",
"多米尼加共和国",
"俄罗斯",
"厄瓜多尔",
"厄立特里亚",
"法国",
"法罗群岛",
"法属波利尼西亚",
"法属圭亚那",
"法属南部领地",
"梵蒂冈",
"菲律宾",
"斐济",
"芬兰",
"佛得角",
"弗兰克群岛",
"冈比亚",
"刚果",
"刚果民主共和国",
"哥伦比亚",
"哥斯达黎加",
"格恩西岛",
"格林纳达",
"格陵兰",
"古巴",
"瓜德罗普",
"关岛",
"圭亚那",
"哈萨克斯坦",
"海地",
"韩国",
"荷兰",
"荷属安地列斯",
"赫德和麦克唐纳群岛",
"洪都拉斯",
"基里巴斯",
"吉布提",
"吉尔吉斯斯坦",
"几内亚",
"几内亚比绍",
"加拿大",
"加纳",
"加蓬",
"柬埔寨",
"捷克共和国",
"津巴布韦",
"喀麦隆",
"卡塔尔",
"开曼群岛",
"科科斯群岛",
"科摩罗",
"科特迪瓦",
"科威特",
"克罗地亚",
"肯尼亚",
"库克群岛",
"拉脱维亚",
"莱索托",
"老挝",
"黎巴嫩",
"利比里亚",
"利比亚",
"立陶宛",
"列支敦士登",
"留尼旺岛",
"卢森堡",
"卢旺达",
"罗马尼亚",
"马达加斯加",
"马尔代夫",
"马耳他",
"马拉维",
"马来西亚",
"马里",
"马其顿",
"马绍尔群岛",
"马提尼克",
"马约特岛",
"曼岛",
"毛里求斯",
"毛里塔尼亚",
"美国",
"美属萨摩亚",
"美属外岛",
"蒙古",
"蒙特塞拉特",
"孟加拉",
"密克罗尼西亚",
"秘鲁",
"缅甸",
"摩尔多瓦",
"摩洛哥",
"摩纳哥",
"莫桑比克",
"墨西哥",
"纳米比亚",
"南非",
"南极洲",
"南乔治亚和南桑德威奇群岛",
"瑙鲁",
"尼泊尔",
"尼加拉瓜",
"尼日尔",
"尼日利亚",
"纽埃",
"挪威",
"诺福克",
"帕劳群岛",
"皮特凯恩",
"葡萄牙",
"乔治亚",
"日本",
"瑞典",
"瑞士",
"萨尔瓦多",
"萨摩亚",
"塞尔维亚,黑山",
"塞拉利昂",
"塞内加尔",
"塞浦路斯",
"塞舌尔",
"沙特阿拉伯",
"圣诞岛",
"圣多美和普林西比",
"圣赫勒拿",
"圣基茨和尼维斯",
"圣卢西亚",
"圣马力诺",
"圣皮埃尔和米克隆群岛",
"圣文森特和格林纳丁斯",
"斯里兰卡",
"斯洛伐克",
"斯洛文尼亚",
"斯瓦尔巴和扬马廷",
"斯威士兰",
"苏丹",
"苏里南",
"所罗门群岛",
"索马里",
"塔吉克斯坦",
"泰国",
"坦桑尼亚",
"汤加",
"特克斯和凯克特斯群岛",
"特里斯坦达昆哈",
"特立尼达和多巴哥",
"突尼斯",
"图瓦卢",
"土耳其",
"土库曼斯坦",
"托克劳",
"瓦利斯和福图纳",
"瓦努阿图",
"危地马拉",
"维尔京群岛，美属",
"维尔京群岛，英属",
"委内瑞拉",
"文莱",
"乌干达",
"乌克兰",
"乌拉圭",
"乌兹别克斯坦",
"西班牙",
"希腊",
"新加坡",
"新喀里多尼亚",
"新西兰",
"匈牙利",
"叙利亚",
"牙买加",
"亚美尼亚",
"也门",
"伊拉克",
"伊朗",
"以色列",
"意大利",
"印度",
"印度尼西亚",
"英国",
"英属印度洋领地",
"约旦",
"越南",
"赞比亚",
"泽西岛",
"乍得",
"直布罗陀",
"智利",
"中非共和国"
};
        /// <summary>
        /// IP地址正则验证
        /// </summary>
        private static Regex _ipAddressRegex = new(@"(\b(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9]))\b)", RegexOptions.Compiled);

        private readonly SemaphoreSlim _initLock = new(initialCount: 1, maxCount: 1);

        private readonly object _versionLock = new();

        private readonly long _loopbackIP = IpToLong("127.0.0.1");

        private readonly QQWryOptions _qqwryOptions;

        /// <summary>
        /// 数据库 缓存
        /// </summary>
        private byte[] _qqwryDbBytes;

        /// <summary>
        /// Ip索引 缓存
        /// </summary>
        private long[] _ipIndexCache;

        /// <summary>
        /// 起始定位
        /// </summary>
        private long _startPosition;

        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool? _init;

        private int? _ipCount;

        private string _version;

        /// <inheritdoc />
        /// <summary>
        /// 记录总数
        /// </summary>
        public int IpCount
        {
            get
            {
                if (!_ipCount.HasValue)
                {
                    Init();
                    _ipCount = _ipIndexCache.Length;
                }

                return _ipCount.Value;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 版本信息
        /// </summary>
        public string Version
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_version))
                {
                    return _version;
                }
                lock (_versionLock)
                {
                    if (!string.IsNullOrWhiteSpace(_version))
                    {
                        return _version;
                    }
                    _version = GetIpLocation("255.255.255.255").Area;
                    return _version;
                }
            }
        }

        static QQWryIpSearch()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _encodingGb2312 = Encoding.GetEncoding("GB2312");
        }

        public QQWryIpSearch(QQWryOptions options)
        {
            _qqwryOptions = options;
        }

        /// <inheritdoc />
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public virtual bool Init()
        {
            if (_init != null)
            {
                return _init.Value;
            }
            _initLock.Wait();
            try
            {
                if (_init != null)
                {
                    return _init.Value;
                }

                EnsureFileExist(_qqwryOptions.DbPath);

                _qqwryDbBytes = FileToBytes(_qqwryOptions.DbPath);

                _ipIndexCache = BlockToArray(ReadIpBlock(_qqwryDbBytes, out _startPosition));

                _ipCount = null;
                _version = null;
                _init = true;
            }
            finally
            {
                _initLock.Release();
            }

            if (_qqwryDbBytes == null)
            {
                throw new InvalidOperationException("无法打开IP数据库" + _qqwryOptions.DbPath + "！");
            }

            return true;

        }

        /// <inheritdoc />
        /// <summary>
        ///  获取指定IP所在地理位置
        /// </summary>
        /// <param name="strIp">要查询的IP地址</param>
        /// <returns></returns>
        public virtual IpLocation GetIpLocation(string strIp)
        {
            var loc = new IpLocation
            {
                Ip = strIp
            };

            var ip = IpToLong(strIp);

            if (ip == _loopbackIP)
            {
                loc.Country = "本机内部环回地址";
                loc.Area = string.Empty;
                return loc;
            }

            if (!Init())
            {
                return loc;
            }

            var iplocation = ReadLocation(loc, ip, _startPosition, _ipIndexCache, _qqwryDbBytes);
            foreach (string isp in isparr)
            {
                if (iplocation.Area.IndexOf(isp) >= 0)
                {
                    iplocation.Isp = isp;
                    break;
                }
            }
            if (string.IsNullOrEmpty(iplocation.Isp))
            {
                iplocation.Isp = "";
            }
            bool isChina = false;
            string tmppro = "";
            string tmpcity = "";
            string tmpcounty = "";
            foreach (string pro in isprovince)
            {
                if (iplocation.Country.IndexOf(pro) >= 0)
                {
                    tmppro = pro;
                    isChina = true;

                    break;
                }
            }
            if (isChina)
            {
                string[] splitCity;
                if (citydirectly.Contains(tmppro))
                {
                    tmppro = tmppro + "市";
                    //中国直辖市
                    splitCity = iplocation.Country.Split("市");

                }
                else
                {
                    var splitPro = iplocation.Country.Split("省");
                    if (splitPro.Length == 1)
                    {
                        var stttt = iplocation.Country.Substring(tmppro.Length);
                        splitCity = stttt.Split("市");
                        if (splitCity.Length == 1)
                        {
                            splitCity = stttt.Split("州");
                        }
                    }
                    else
                    {
                        tmppro = splitPro[0] + "省";
                        splitCity = splitPro[1].Split("市");
                    }
                }
                tmpcity = splitCity[0] + "市";
                if (splitCity[1].IndexOf("县") >= 0)
                {
                    tmpcounty = splitCity[1].Split("县")[0] + "县";
                }
                else if (splitCity[1].IndexOf("区") >= 0)
                {
                    tmpcounty = splitCity[1].Split("区")[0] + "区";
                }

                iplocation.Country = "中国";
                iplocation.Province = tmppro;
                iplocation.City = tmpcity;
                iplocation.County = tmpcounty;
            }
            else
            {
                string tmpcc = "";
                foreach (var cc in allct)
                {
                    if (iplocation.Country.StartsWith(cc))
                    {
                        tmpcc = cc;
                        string tmpcceee = iplocation.Country.Substring(cc.Length);
                        string[] zzsplit = tmpcceee.Split("州");
                        if (zzsplit.Length > 1)
                        {
                            tmppro = zzsplit[0] + "州";
                            tmpcity = zzsplit[1];
                        }
                        else
                        {
                            zzsplit = tmpcceee.Split("郡");
                            if (zzsplit.Length > 1)
                            {
                                tmppro = zzsplit[0] + "郡";
                                tmpcity = zzsplit[1];
                            }
                            else
                            {
                                zzsplit = tmpcceee.Split("邦");
                                if (zzsplit.Length > 1)
                                {
                                    tmppro = zzsplit[0] + "邦";
                                    tmpcity = zzsplit[1];
                                }
                            }

                        }

                        break;
                    }
                }

                iplocation.Country = tmpcc;
                iplocation.Province = tmppro;
                iplocation.City = tmpcity;
                iplocation.County = tmpcounty;
            }


            return iplocation;
        }

        /// <inheritdoc />
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<bool> InitAsync(CancellationToken token = default)
        {
            if (_init != null)
            {
                return _init.Value;
            }

            await _initLock.WaitAsync(token);

            try
            {
                if (_init != null)
                {
                    return _init.Value;
                }


                EnsureFileExist(_qqwryOptions.DbPath);

                _qqwryDbBytes = FileToBytes(_qqwryOptions.DbPath);

                _ipIndexCache = BlockToArray(ReadIpBlock(_qqwryDbBytes, out _startPosition));

                _ipCount = null;
                _version = null;
                _init = true;
            }
            finally
            {
                _initLock.Release();
            }

            if (_qqwryDbBytes == null)
            {
                throw new InvalidOperationException("无法打开IP数据库" + _qqwryOptions.DbPath + "！");
            }

            return true;
        }

        /// <inheritdoc />
        /// <summary>
        ///  获取指定IP所在地理位置
        /// </summary>
        /// <param name="strIp">要查询的IP地址</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<IpLocation> GetIpLocationAsync(string strIp, CancellationToken token = default)
        {
            var loc = new IpLocation
            {
                Ip = strIp
            };

            long ip = IpToLong(strIp);
            if (ip == _loopbackIP)
            {
                loc.Country = "本机内部环回地址";
                loc.Area = string.Empty;
                return loc;
            }
            if (!await InitAsync(token))
            {
                return loc;
            }
            return ReadLocation(loc, ip, _startPosition, _ipIndexCache, _qqwryDbBytes);
        }

        /// <inheritdoc />
        /// <summary>
        /// 检查IP合法性
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool CheckIp(string ip)
        {
            return _ipAddressRegex.IsMatch(ip);
        }

        /// <inheritdoc />
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            _initLock?.Dispose();
            _qqwryDbBytes = null;
            _qqwryDbBytes = null;
            _ipIndexCache = null;
            _init = null;
        }

        ///<summary>
        /// 将字符串形式的IP转换位long
        ///</summary>
        ///<param name="strIp"></param>
        ///<returns></returns>
        private static long IpToLong(string strIp)
        {
            var ipBytes = new byte[8];
            var strArr = strIp.Split(new char[] { '.' });
            for (var i = 0; i < 4; i++)
            {
                ipBytes[i] = byte.Parse(strArr[3 - i]);
            }
            return BitConverter.ToInt64(ipBytes, 0);
        }

        ///<summary>
        /// 将索引区字节块中的起始IP转换成Long数组
        ///</summary>
        ///<param name="ipBlock"></param>
        private static long[] BlockToArray(byte[] ipBlock)
        {
            var ipArray = new long[ipBlock.Length / 7];
            var ipIndex = 0;
            var temp = new byte[8];
            for (var i = 0; i < ipBlock.Length; i += 7)
            {
                Array.Copy(ipBlock, i, temp, 0, 4);
                ipArray[ipIndex] = BitConverter.ToInt64(temp, 0);
                ipIndex++;
            }
            return ipArray;
        }

        /// <summary>
        ///  从IP数组中搜索指定IP并返回其索引
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="ipArray">IP数组</param>
        /// <param name="start">指定搜索的起始位置</param>
        /// <param name="end">指定搜索的结束位置</param>
        /// <returns></returns>
        private static int SearchIp(long ip, long[] ipArray, int start, int end)
        {
            //二分法 https://baike.baidu.com/item/%E4%BA%8C%E5%88%86%E6%B3%95%E6%9F%A5%E6%89%BE
            while (true)
            {
                //计算中间索引
                var middle = (start + end) / 2;
                if (middle == start)
                {
                    return middle;
                }
                else if (ip < ipArray[middle])
                {
                    end = middle;
                }
                else
                {
                    start = middle;
                }
            }
        }

        ///<summary>
        /// 读取IP文件中索引区块
        ///</summary>
        ///<returns></returns>
        private static byte[] ReadIpBlock(byte[] bytes, out long startPosition)
        {
            long offset = 0;
            startPosition = ReadLongX(bytes, offset, 4);
            offset += 4;
            var endPosition = ReadLongX(bytes, offset, 4);
            offset = startPosition;
            var count = (endPosition - startPosition) / 7 + 1;//总记录数

            var ipBlock = new byte[count * 7];
            for (var i = 0; i < ipBlock.Length; i++)
            {
                ipBlock[i] = bytes[offset + i];
            }
            return ipBlock;
        }

        /// <summary>
        ///  从IP文件中读取指定字节并转换位long
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="bytesCount">需要转换的字节数，主意不要超过8字节</param>
        /// <returns></returns>
        private static long ReadLongX(byte[] bytes, long offset, int bytesCount)
        {
            var cBytes = new byte[8];
            for (var i = 0; i < bytesCount; i++)
            {
                cBytes[i] = bytes[offset + i];
            }
            return BitConverter.ToInt64(cBytes, 0);
        }

        /// <summary>
        ///  从IP文件中读取字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="flag">转向标志</param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static string ReadString(byte[] bytes, int flag, ref long offset)
        {
            if (flag == 1 || flag == 2)//转向标志
            {
                offset = ReadLongX(bytes, offset, 3);
            }
            else
            {
                offset -= 1;
            }
            var list = new List<byte>();
            var b = (byte)bytes[offset];
            offset += 1;
            while (b > 0)
            {
                list.Add(b);
                b = (byte)bytes[offset];
                offset += 1;
            }
            return _encodingGb2312.GetString(list.ToArray());
        }

        private static byte[] FileToBytes(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] bytes = new byte[fileStream.Length];

                fileStream.Read(bytes, 0, bytes.Length);

                fileStream.Close();

                return bytes;
            }
        }

        private static void EnsureFileExist(string ipDbPath)
        {
            var dir = Path.GetDirectoryName(ipDbPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir ?? throw new InvalidOperationException());
            }
            if (!File.Exists(ipDbPath))
            {
                throw new Exception($"无法找到IP数据库{ipDbPath}");
            }

        }

        private static IpLocation ReadLocation(IpLocation loc, long ip, long startPosition, long[] ipIndex, byte[] qqwryDbBytes)
        {
            long offset = SearchIp(ip, ipIndex, 0, ipIndex.Length) * 7 + 4;

            //偏移
            var arrayOffset = startPosition + offset;
            //跳过结束IP
            arrayOffset = ReadLongX(qqwryDbBytes, arrayOffset, 3) + 4;
            //读取标志
            var flag = qqwryDbBytes[arrayOffset];
            arrayOffset += 1;
            //表示国家和地区被转向
            if (flag == 1)
            {
                arrayOffset = ReadLongX(qqwryDbBytes, arrayOffset, 3);
                //再读标志
                flag = qqwryDbBytes[arrayOffset];
                arrayOffset += 1;
            }
            var countryOffset = arrayOffset;
            loc.Country = ReadString(qqwryDbBytes, flag, ref arrayOffset);

            if (flag == 2)
            {
                arrayOffset = countryOffset + 3;
            }

            flag = qqwryDbBytes[arrayOffset];
            arrayOffset += 1;
            loc.Area = ReadString(qqwryDbBytes, flag, ref arrayOffset);

            if (" CZ88.NET".Equals(loc.Area, StringComparison.CurrentCultureIgnoreCase))
            {
                loc.Area = string.Empty;
            }

            return loc;
        }

    }

    public class IpLocation
    {
        public string Ip, Country, Province, City, County, Area, Isp;
    }
    public class QQWryOptions
    {
        public QQWryOptions()
        {

        }

        public QQWryOptions(string dbPath)
        {
            DbPath = dbPath;
        }

        private string _dbPath = string.Empty;

        /// <summary>
        /// DbPath
        /// </summary>
        public string DbPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_dbPath))
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qqwry.dat");
                }
                else
                {
                    return _dbPath;
                }
            }
            set
            {
                _dbPath = value;
            }
        }
    }
    public interface IIpSearch
    {
        /// <summary>
        /// 数据库IP数量
        /// </summary>
        int IpCount { get; }

        /// <summary>
        /// 数据库版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 检查是否是IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        bool CheckIp(string ip);

        /// <summary>
        /// 获取IP信息
        /// </summary>
        /// <param name="strIp"></param>
        /// <returns></returns>
        IpLocation GetIpLocation(string strIp);

        /// <summary>
        /// 检查是否是IP地址
        /// </summary>
        /// <param name="strIp"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IpLocation> GetIpLocationAsync(string strIp, CancellationToken token = default);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        bool Init();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> InitAsync(CancellationToken token = default);
    }
}
