using MyAccess.DB.Attr;
using MyAccess.DB.Builder.UpdateToSql;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class UpdateColumnsBuilder<X> : AbstractBuilder<UpdateColumnsBuilder<X>>
    {
        private List<string> _setColumns = new List<string>();
        public UpdateColumnsBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }

        protected override UpdateColumnsBuilder<X> This()
        {
            return this;
        }
        public UpdateColumnsBuilder<X> SetColum<Result1>(Expression<Func<X, Result1>> colum, Expression<Func<X, Result1>> val)
        {
            string tmpname = ExpressionTool.GetMemberName(colum);
            string tmpval = UpdateExpressionToString.Convert(val);
            _setColumns.Add($"{tmpname}={tmpval}");
            return this;
        }
        public UpdateColumnsBuilder<X> Where(Expression<Func<X, bool>> expression)
        {
            Type EntityType = typeof(X);
            TableNameAttribute tn = EntityType.GetCustomAttribute<TableNameAttribute>();
            string tablename = tn == null ? EntityType.Name : tn.Name;
            string where = this._sqlBuilder.GetWhereByLambda(expression);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _setColumns.Count; i++)
            {
                sb.Append($",{_setColumns[i]}");
            }
            string updatestr = sb.ToString();
            if (updatestr.StartsWith(","))
            {
                updatestr = updatestr.Substring(1);
            }


            _sqlBuilder.AppendDiv();
            _sqlBuilder.Append($"update {tablename} set {updatestr} where {where}");
            return this;
        }
        public int Do()
        {
            return _sqlBuilder.Do<DoExecSql>().RowCount;
        }
        public async Task<int> DoAsync()
        {
            return (await _sqlBuilder.DoAsync<DoExecSql>()).RowCount;
        }
    }
}
