using System;
using System.Reflection;

namespace MyAccess.Aop
{
    public interface IBaseEntity
    {
        bool EnableRecord();
        void StartRecord();
        void AddProperty(PropertyInfo pi);
        PropertyInfo[] GetUsedPropertys();
    }
}
