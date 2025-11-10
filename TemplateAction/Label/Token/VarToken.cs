using System;
namespace TemplateAction.Label.Token
{
    /// <summary>
    /// 以$开头的变量,例：$hell.row
    /// </summary>
    public class VarToken : BaseToken
    {
        public const string TOKEN_TYPE = "VARNAME";
        public override string Type
        {
            get { return TOKEN_TYPE; }
        }
        public VarToken(string val)
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

        public override object GetValue()
        {
            return mValue.Substring(1);
        }
    }
}
