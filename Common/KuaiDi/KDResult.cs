using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.KuaiDi
{
    public class KDResult<T>
    {
        public bool IsSuccess { get; set; }
        public string ErrMsg { get; set; }
        public T Data { get; set; }
    }
}
