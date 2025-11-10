using System.Threading.Tasks;

namespace MyAccess.DB
{
    public class DoExecReturnIdentity : DoExecSql
    {
        public long LastInsertedId { get; set; }
        private SqlBuilder _sql;
        private string _idname;
        public DoExecReturnIdentity(SqlBuilder sqlBuilder, string idname)
        {
            _sql = sqlBuilder;
            _idname = idname;
        }
        public override void Excute(ExcuteParam p)
        {
            p.Sql = _sql;
            LastInsertedId = _sql.Comparable.DoExecReturnIdentity(p, _idname);
        }
        public override async Task ExcuteAsync(ExcuteParam p)
        {
            p.Sql = _sql;
            LastInsertedId = await _sql.Comparable.DoExecReturnIdentityAsync(p, _idname);
        }
    }
}
