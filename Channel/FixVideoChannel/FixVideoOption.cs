using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixVideoChannel
{
    public class FixVideoOption
    {
        public string event_conn { get; set; }
        public string redis_conn { get; set; }
        public string ffmpeg_path { get; set; }
    }
}
