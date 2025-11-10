using System;

namespace TemplateAction.Core
{
    public interface IEventRegister
    {
        void Register(string key, ITAEventHandler handler);
        void RegisterReponse(string key, ITAResponseEventHandler handler);

    }
}
