using System;
using System.Collections.Generic;
using TemplateAction.Common;

namespace TemplateAction.Label
{
    public class TAParams
    {
        protected Dictionary<string, object> mParamList = new Dictionary<string, object>();

        public Dictionary<string, object> ParamList
        {
            get { return mParamList; }
        }

        #region 参数操作
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParam(string key, string value)
        {
            mParamList[key] = value;
        }
        public void AddParam(string key, object value)
        {
            mParamList[key] = value;
        }
        public bool TryGetParam(string key,out object result)
        {
            return mParamList.TryGetValue(key, out result);
        }
     
        public T GetParam<T>(string key, T def)
        {
            object result;
            if(mParamList.TryGetValue(key, out result))
            {
                return TAConverter.Cast<T>(result, def);
            }
            else
            {
                return def;
            }
        }
        public int ParamCount
        {
            get { return mParamList.Count; }
        }
        #endregion
    }
}
