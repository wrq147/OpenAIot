using System;


namespace TemplateAction.Core
{
    public interface ITACookie
    {
        DateTime Expires { get; set; }
        bool Secure { get; set; }
        bool HttpOnly { get; set; }
        string Path { get; set; }
        string Domain { get; set; }
        void SetValue(string value);
        string GetValue();
        bool IsEmpty();
    }
}
