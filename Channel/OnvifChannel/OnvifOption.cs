using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnvifChannel
{
    public class OnvifOption
    {
        public string event_conn { get; set; }
        public string event_user { get; set; }
        public string event_pass { get; set; }
        public string redis_conn { get; set; }
        public string node_id { get; set; }
        public string minio_server { get; set; }
        public string minio_access { get; set; }
        public string minio_secret { get; set; }
        public string minio_bucket { get; set; }
        public string minio_url { get; set; }
    }
}
