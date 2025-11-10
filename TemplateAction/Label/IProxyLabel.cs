using System;


namespace TemplateAction.Label
{
    public interface IProxyLabel
    {
        IProxyLabel Parent();
        T GetParam<T>(string key, T def);
        bool TryGetParam(string key, out object result, bool calexp = true);
        int ParamCount { get; }
    }
}
