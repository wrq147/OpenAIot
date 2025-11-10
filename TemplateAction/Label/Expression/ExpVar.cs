using System;
using System.Collections;
using System.Reflection;

namespace TemplateAction.Label.Expression
{
    public class ExpVar : IExpressionLinkItem
    {
        private string _varname;
        /// <summary>
        /// 用来判断是否为取变量索引操作
        /// </summary>
        private TemplateExpress _idx;
        public ExpVar(string varname)
        {
            _varname = varname;
        }
        public void InitIdx(TemplateExpress idx)
        {
            _idx = idx;
        }
 

        private object ArrIdx(ITemplateContext context, object arr)
        {
            IList arrt = arr as IList;
            if (arrt == null)
            {
                Type at = arr.GetType();
                MethodInfo setValueMethod = at.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
                return setValueMethod.Invoke(arr, new object[] { _idx.Calculate(context).Value });
            }
            else
            {
                int ix = Common.TAConverter.Cast(_idx.Calculate(context).Value, 0);
                return arrt[ix];
            }
        }
        private void ArrIdxAssign(ITemplateContext context, object arr, object result)
        {
            IList arrt = arr as IList;
            if (arrt == null)
            {
                Type at = arr.GetType();
                MethodInfo setValueMethod = at.GetMethod("set_Item", BindingFlags.Instance | BindingFlags.Public);
                setValueMethod.Invoke(arr, new object[] { _idx.Calculate(context).Value });
            }
            else
            {
                int ix = Common.TAConverter.Cast(_idx.Calculate(context).Value, 0);
                arrt[ix] = result;
            }
        }
        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scope"></param>
        /// <param name="result"></param>
        public void Assign(ITemplateContext context, object scope, object result)
        {
            if (scope == null)
            {
                if (_idx == null)
                {
                    context.PushGlobal(_varname, result);
                }
                else
                {
                    object arr = context.GetGlobal(_varname);
                    ArrIdxAssign(context, arr, result);
                }
            }
            else
            {
                Type nextType = scope.GetType();
                PropertyInfo pi = nextType.GetProperty(_varname, BindingFlags.Public | BindingFlags.Instance);
                if (pi == null)
                {
                    throw new Exception("对象属性不存在");
                }
                if (_idx == null)
                {
                    pi.SetValue(scope, result);
                }
                else
                {
                    object rawobj = pi.GetValue(scope, null);
                    ArrIdxAssign(context, rawobj, result);
                }
            }
        }
        public TAVar Calculate(ITemplateContext context, object scope)
        {
            //转换变量
            if (scope == null)
            {
                if (_idx == null)
                {
                    return new TAVar(context.GetGlobal(_varname), VarType.Var);
                }
                else
                {
                    object arr = context.GetGlobal(_varname);
                    return new TAVar(ArrIdx(context, arr), VarType.Var);
                }
            }
            else
            {
                Type nextType = scope.GetType();
                PropertyInfo pi = nextType.GetProperty(_varname, BindingFlags.Public | BindingFlags.Instance);
                if (pi == null)
                {
                    return new TAVar(null, VarType.Keyword);
                }
                object rawobj = pi.GetValue(scope, null);
                if (_idx == null)
                {
                    return new TAVar(rawobj, VarType.Var);
                }
                else
                {
                    return new TAVar(ArrIdx(context, rawobj), VarType.Var);
                }
            }
        }
    }
}
