using System;

namespace TemplateAction.Label.Expression
{
    /// <summary>
    /// 表达式变量与返回值
    /// </summary>
    public class TAVar
    {
        private object _val;
        private VarType _type;

        public TAVar(object var, VarType t)
        {
            _val = var;
            _type = t;
        }
        public override bool Equals(object obj)
        {
            return _val.Equals(obj);
        }
        public override int GetHashCode()
        {
            return _val.GetHashCode();
        }
        public bool ToBoolean()
        {
            if (_val == null) return false;
            if (_type == VarType.Var)
            {
                return _val is bool ? (bool)_val : true;
            }
            return Convert.ToBoolean(_val);
        }
        public object Value
        {
            get { return _val; }
        }
        public Type ValueType
        {
            get { return _val == null ? null : _val.GetType(); }
        }
        public bool IsNull()
        {
            return _val == null;
        }
    }
    /// <summary>
    /// 变量分类
    /// </summary>
    public enum VarType
    {
        Number = 0,
        Var = 1,
        Keyword = 2,
        String = 3
    }
}
