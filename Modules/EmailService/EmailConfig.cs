using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailConfig
    {
        /// <summary>
        /// email用户名
        /// </summary>
        public string email_from_name { get; set; }
        /// <summary>
        /// email地址
        /// </summary>
        public string email_from { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string email_password { get; set; }
        /// <summary>
        /// 使用的ssl：0为不使用、1为自动、2为SslOnConnect、3为StartTls
        /// </summary>
        public int ssl { get; set; }
        /// <summary>
        /// email主机
        /// </summary>
        public string email_host { get; set; }
        /// <summary>
        /// email端口
        /// </summary>
        public int email_post { get; set; }
        /// <summary>
        /// email绑定的标题
        /// </summary>
        public string email_bind_title { get; set; }
    }
}
