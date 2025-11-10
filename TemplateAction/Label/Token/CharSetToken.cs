using System;

namespace TemplateAction.Label.Token
{
    /// <summary>
    /// 名称标记，用来表示标签中的Fun,If这些字母或数字,首字符必需为字母
    /// </summary>
    public class CharSetToken : BaseToken
    {
        public const string TOKEN_TYPE = "CharSet";
        public const string NULL = "null";
        public const string TRUE = "true";
        public const string FALSE = "false";
        /// <summary>
        /// 表示当前Context
        /// </summary>
        public const string THIS = "this";
        public const string IF = "if";
        public const string ELSE = "else";
        public const string FOR = "for";
        public const string WHILE = "while";
        public const string END = "end";
        public const string IN = "in";
        public const string BREAK = "break";
        public override object GetValue()
        {
            return mValue;
        }
        public CharSetToken(string val)
            : base(val)
        {
        }
        public override bool Append(char value)
        {
            if (char.IsLetterOrDigit(value) || value.Equals('_'))
            {
                mValue += value;
                return true;
            }
            return false;
        }

        public override string Type
        {
            get { return TOKEN_TYPE; }
        }
    }

}