using System;

namespace TemplateAction.Label.Token
{
    /// <summary>
    /// 实数标记
    /// </summary>
    public class NumberToken : BaseToken
    {
        private bool _isFloat = false;
        public const string TOKEN_TYPE = "Number";

        public override object GetValue()
        {
            if (_isFloat)
            {
                double rt;
                if (double.TryParse(mValue, out rt))
                {
                    return rt;
                }
                return 0;
            }
            else
            {
                long rt;
                if (long.TryParse(mValue, out rt))
                {
                    return rt;
                }
                return 0;
            }
        }
        public NumberToken(string val)
            : base(val) { }
        public override bool Append(char value)
        {
            if (char.IsDigit(value))
            {
                mValue += value;
                return true;
            }
            else if(value == '.')
            {
                _isFloat = true;
                mValue += value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string Type
        {
            get { return TOKEN_TYPE; }
        }
    }
}