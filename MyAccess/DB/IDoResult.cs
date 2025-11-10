using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public interface IDoResult<T>
    {
        void SetNavigate(List<string> sub, List<Type> tMaps);
        void SetResult(T result);
        Task SetResultAsync(T result);
    }
}
