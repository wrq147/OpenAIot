using System.Collections.Generic;
using System.Reflection;

namespace MyAccess.Aop
{
    public class BaseEntity : IBaseEntity
    {
        protected bool mEnableRecord = false;
        /// <summary>
        /// 有更改的属性列表
        /// </summary>
        protected List<PropertyInfo> usedPropertys = new List<PropertyInfo>();
        public void AddProperty(PropertyInfo pi)
        {
            if (!usedPropertys.Contains(pi))
            {
                usedPropertys.Add(pi);
            }
        }

        public bool EnableRecord()
        {
            return mEnableRecord;
        }

        public PropertyInfo[] GetUsedPropertys()
        {
            return usedPropertys.ToArray();
        }

        public void StartRecord()
        {
            mEnableRecord = true;
        }
    }
}
