using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    public class SearchObjectParam
    {
        public SearchPageParam Query { get; set; }
        public Data_ServerTokenInfo User { get; set; }
    }
}
