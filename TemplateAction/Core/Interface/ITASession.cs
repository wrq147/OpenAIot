using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public interface ITASession
    {
        string SessionId { get; }
        IEnumerable<string> Keys { get; }
        bool Remove(string name);
        void SetObject(string name, object value);
        void SetInt32(string name, int value);
        void SetString(string name, string value);
        int? GetInt32(string name);
        string GetString(string name);
        T GetObject<T>(string name) where T : class;
    }
}
