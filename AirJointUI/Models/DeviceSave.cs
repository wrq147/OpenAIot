using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class DeviceSave
    {
        public string Id { get; set; }
        /// <summary>
        /// 是否联控中
        /// </summary>
        public bool IsControl { get; set; }
        /// <summary>
        /// 是否运行中
        /// </summary>
        public bool IsRunning { get; set; }
    }
}
