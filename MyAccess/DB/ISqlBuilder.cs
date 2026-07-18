using MyAccess.DB.Builder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public interface ISqlBuilder<X> where X : ISqlBuilder<X>
    {
        X Append(string value);
        X AppendLeft(string value);
        X AppendParam<T>(List<T> inlist);
        X AppendParam<T>(T[] inarr);
        X AppendParam(object value);
        X Then(bool condition, Action<X> config);

        InsertBuilder<T> Insert<T>(T inserted);
        InsertBuilder<T> Insert<T>(List<T> inserted);
        InsertBuilder<T> Insert<T>(T[] inserted);

        UpdateBuilder<T> Update<T>(T updated, string where = "");
        DeleteBuilder<T> Delete<T>(string where);
        DeleteBuilder<T> Delete<T>();


        T Do<T>() where T : IDoCommand, new();
        Task<T> DoAsync<T>() where T : IDoCommand, new();
    }
}
