using System;
using System.Collections.Generic;
namespace Common.Share
{
    public class GeneralOption
    {
        /// <summary>
        /// 当前站点域名
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 数据库类型：MySql（默认）、Sqlite
        /// </summary>
        public string sqltype { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string connstr { get; set; }
        /// <summary>
        /// redis连接字符串
        /// </summary>
        public string redisconn { get; set; }
        /// <summary>
        /// 是否开启快速启动（为true将不升级迁移数据库和启用定时器）
        /// </summary>
        public bool quick_init { get; set; }
        /// <summary>
        /// 当前工作站编号（Id生成需要）
        /// </summary>
        public ushort idgenerator_workid { get; set; }
        /// <summary>
        /// 限制文件大小（单位MB)
        /// </summary>
        public int limit_file_size { get; set; }
        /// <summary>
        /// 限制上传的文件类型
        /// </summary>
        public string[] limit_file_type { get; set; }
        /// <summary>
        /// MinIO文件服务器
        /// </summary>
        public string minio_server { get; set; }
        /// <summary>
        /// MinIO访问密钥
        /// </summary>
        public string minio_access{ get; set; }
        /// <summary>
        /// MinIO密钥
        /// </summary>
        public string minio_secret{ get; set; }
        /// <summary>
        /// MinIO的桶
        /// </summary>
        public string minio_bucket { get; set; }
        /// <summary>
        /// MinIO文件访问地址
        /// </summary>
        public string minio_url { get; set; }
        /// <summary>
        /// 图片是否生成缩略图（传false时，可以通过nginx实时生成缩略图）
        /// </summary>
        public bool enable_thumb { get; set; }
        /// <summary>
        /// 默认图片url
        /// </summary>
        public string default_imgurl { get; set; }
        /// <summary>
        /// 默认头像url
        /// </summary>
        public string default_avatar { get; set; }

        /// <summary>
        /// 登录是否需要验证码
        /// </summary>
        public bool login_need_code { get; set; }
        /// <summary>
        /// 令牌过期时间，单位分钟
        /// </summary>
        public int expire_minutes { get; set; }
        /// <summary>
        /// 加密密钥
        /// </summary>
        public string secret_key { get; set; }
        /// <summary>
        /// 令牌验证api
        /// </summary>
        public string token_url { get; set; }
        /// <summary>
        /// 数据权限api
        /// </summary>
        public string scope_url { get; set; }
        /// <summary>
        /// 是否启用简单模式登录（访问令牌永不过期，不需要刷新令牌）
        /// </summary>
        public bool enable_simple_token { get; set; }
        /// <summary>
        /// 事件总线连接字符串
        /// </summary>
        public string event_bus_conn { get; set; }
        /// <summary>
        /// 临时文件上传key
        /// </summary>
        public string tmp_filekey { get; set; }

    }
}
