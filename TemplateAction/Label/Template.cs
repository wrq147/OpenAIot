using System.Collections.Generic;
namespace TemplateAction.Label
{
    /// <summary>
    /// 模板基类
    /// </summary>
    public abstract class Template : ILabel
    {
        protected Template mParent;
        protected TAParams mParam;
        protected Template()
        {
            mParam = new TAParams();
        }
        public TAParams Param
        {
            get { return mParam; }
        }
        public ILabel Parent
        {
            get { return mParent; }
        }

        protected List<Template> mChilds = new List<Template>();


        public void AddLable(Template label)
        {
            label.mParent = this;
            mChilds.Add(label);
        }
        public void RemoveLable(Template label)
        {
            label.mParent = null;
            mChilds.Remove(label);
        }

        /// <summary>
        /// 获取最顶部标签
        /// </summary>
        /// <returns></returns>
        public ILabel GetTopestTemplate()
        {
            ILabel tTemp = this;
            while (tTemp.Parent != null)
            {
                tTemp = tTemp.Parent;
            }
            return tTemp;
        }

        public string MakeHtml(ITemplateContext context)
        {
            IProxyLabel preLabel = context.CurrentLabel;
            context.CurrentLabel = new ProxyLabel(context, this);
            string rt = OnMake(context);
            context.CurrentLabel = preLabel;
            return rt;
        }
        protected virtual void OnPreMakeHtml() { }
        /// <summary>
        /// 生成最终结果
        /// </summary>
        /// <param name="cn"></param>
        protected abstract string OnMake(ITemplateContext context);

        public abstract string Type { get; }
    }
}