using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    public class BLLDbStore
    {
        public DbHelp ThreadDb { get; set; }
        public long StoreId { get; set; }
    }
}
