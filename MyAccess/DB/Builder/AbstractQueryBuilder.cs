using MyAccess.DB.Attr;
using System;
using System.Linq;
using System.Reflection;

namespace MyAccess.DB.Builder
{
    public abstract class AbstractQueryBuilder<X> : AbstractBuilder<X> where X : ISqlBuilder<X>
    {
        public AbstractQueryBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        /// <summary>
        /// sql取前几条
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public X Take(int num)
        {
            _sqlBuilder.Comparable.Take(num);
            return This();
        }
        protected string GenerateTable(Type EntityType)
        {
            TableNameAttribute tn = EntityType.GetCustomAttribute<TableNameAttribute>();
            string table = tn == null ? EntityType.Name : tn.Name;
            if (_sqlBuilder.SubMaps != null)
            {
                table = table + " a ";
                var submaplist = _sqlBuilder.SubMaps;
                for (int i = 0; i < submaplist.Count; i++)
                {
                    string tmpprefix = DBMapping.GetSubPrefix(i);

                    Type subType;
                    string onCondi;
                    if (_sqlBuilder.SubTypeMaps[i] != null)
                    {
                        subType = _sqlBuilder.SubTypeMaps[i];
                        onCondi = " on " + _sqlBuilder.SubIdMaps[i];
                    }
                    else
                    {
                        PropertyInfo ptInfo = EntityType.GetProperty(submaplist[i]);
                        subType = ptInfo.PropertyType;
                        PropertyInfo[] properties = subType.GetProperties();
                        string subpropname = string.Empty;
                        foreach (PropertyInfo property in properties)
                        {
                            IDAttribute attribute = property.GetCustomAttribute<IDAttribute>();
                            if (attribute != null && property.IsDefined(typeof(IDAttribute)))
                            {
                                subpropname = property.Name;
                                break;
                            }
                        }
                        onCondi = $" on a.{_sqlBuilder.SubIdMaps[i]}={tmpprefix}.{subpropname}";
                    }


                    TableNameAttribute jointn = subType.GetCustomAttribute<TableNameAttribute>();
                    string jointableName = jointn == null ? subType.Name : jointn.Name;
                    table = table + $" {_sqlBuilder.JoinWays[i]} {jointableName} {tmpprefix}{onCondi}";
                }
            }
            return table;
        }
        protected string GenerateFields(Type EntityType)
        {
            if (_sqlBuilder.SubMaps == null)
            {
                return "*";
            }
            var newSubMaps = _sqlBuilder.SubMaps.Where(x => x != SqlBuilder.NullSub);
            if (newSubMaps.Count() == 0)
            {
                return "*";
            }
            var tmpsign = this._sqlBuilder.Comparable.GetFieldSign();
            string fields = "a.*";
            var submaplist = _sqlBuilder.SubMaps;
            for (int i = 0; i < submaplist.Count; i++)
            {
                string tmpprefix = DBMapping.GetSubPrefix(i);
                PropertyInfo[] properties;
                if (_sqlBuilder.SubTypeMaps[i] != null)
                {
                    properties = _sqlBuilder.SubTypeMaps[i].GetProperties();
                }
                else
                {
                    PropertyInfo ptInfo = EntityType.GetProperty(submaplist[i]);
                    properties = ptInfo.PropertyType.GetProperties();
                }

                foreach (PropertyInfo property in properties)
                {
                    if (property.IsDefined(typeof(DataIgnoreAttribute)) || !DBMapping.IsMapping(property.PropertyType))
                    {
                        continue;
                    }
                    fields = fields + $",{tmpprefix}.{property.Name} {tmpsign}{tmpprefix}.{property.Name}{tmpsign}";
                }
            }
            return fields;
        }
    }
}
