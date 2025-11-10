using System;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public interface IDoCommand
    {
        void Excute(ExcuteParam p);
        Task ExcuteAsync(ExcuteParam p);
    }
}
