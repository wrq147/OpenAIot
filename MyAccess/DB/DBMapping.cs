using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public static class DBMapping
    {
        public static bool IsMapping(Type t)
        {
            if (t.IsEnum)
            {
                return true;
            }
            return _mappings.ContainsKey(t);
        }
        public static string TryGetValue(Type t)
        {
            string mapval;
            if (_mappings.TryGetValue(t, out mapval))
            {
                return mapval;
            }
            return null;
        }
        public static string GetSubPrefix(int i)
        {
            return _subprefix[i];
        }
        public static int GetIndexByPrefix(string prefix)
        {
            switch (prefix)
            {
                case "b": return 0;
                case "c": return 1;
                case "d": return 2;
                case "e": return 3;
                case "f": return 4;
                case "g": return 5;
            }
            return 0;
        }
        private static string[] _subprefix = new string[] { "b", "c", "d", "e", "f", "g" };
        private static IDictionary<Type, string> _mappings = new Dictionary<Type, string>()
        {
             {typeof(DateTime?), "DateTime"},
             {typeof(DateTime), "DateTime"},
             {typeof(decimal?), "decimal"},
             {typeof(decimal), "decimal"},
             {typeof(string), "string"},
             {typeof(Int32?), "Int32"},
             {typeof(Int32), "Int32"},
             {typeof(Int64?), "Int64"},
             {typeof(Int64), "Int64"},
             {typeof(bool?), "bool"},
             {typeof(bool), "bool"},
             {typeof(byte?), "byte"},
             {typeof(byte), "byte"},
             {typeof(char?), "char"},
             {typeof(char), "char"},
             {typeof(short?), "short"},
             {typeof(short), "short"},
             {typeof(double?), "double"},
             {typeof(double), "double"},
             {typeof(float?), "float"},
             {typeof(float), "float"}
        };
    }
}
