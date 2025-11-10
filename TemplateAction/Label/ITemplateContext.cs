using System;
namespace TemplateAction.Label
{
    public interface ITemplateContext
    {
        /// <summary>
        /// 退出循环计算
        /// </summary>
        int BreakCount { get; set; }
        IProxyLabel CurrentLabel { get; set; }
        void PushGlobal(string key, object value);
        void PopGlobal();
        int StackCount();
        T GetGlobal<T>(string key, T def);

        object GetGlobal(string key);
        bool IsDefine(string key);
        string Include(string src);
    }
}
