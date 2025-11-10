using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class OutKFProductPage : BaseQueryParam
    {
        public string Name { get; set; }
        public string[] Pids { get; set; }
    }
}
