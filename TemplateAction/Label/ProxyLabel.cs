using System;

namespace TemplateAction.Label
{
    /// <summary>
    /// 标签代理
    /// </summary>
    public class ProxyLabel : IProxyLabel
    {
        private ILabel mLabel;
        private ITemplateContext mContext;
        private IProxyLabel mParent;
        public ProxyLabel(ITemplateContext context, ILabel tem)
        {
            mContext = context;
            mLabel = tem;
            if (mLabel.Parent != null)
            {
                mParent = new ProxyLabel(mContext, mLabel.Parent);
            }
        }
        public IProxyLabel Parent()
        {
            return mParent;
        }

        public T GetParam<T>(string key, T def)
        {
            return mLabel.Param.GetParam<T>(key, def);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <param name="calexp">是否解释表达式</param>
        /// <returns></returns>
        public bool TryGetParam(string key, out object result, bool calexp = true)
        {
            if (mLabel.Param.TryGetParam(key, out result))
            {
                if (calexp)
                {
                    //如果是表达式则计算
                    TemplateExpress exp = result as TemplateExpress;
                    if (exp != null)
                    {
                        result = exp.Calculate(mContext).Value;
                    }
                }
                return true;
            }
            return false;
        }

        public int ParamCount
        {
            get { return mLabel.Param.ParamCount; }
        }

    }
}
