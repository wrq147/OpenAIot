using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using TemplateAction.Common;

namespace TemplateAction.Label.Expression
{
    public class ExpFun : IExpressionLinkItem
    {
        private LinkedList<TemplateExpress> _params = new LinkedList<TemplateExpress>();
        private string _funname;
        private MethodInfo _m = null;
        public ExpFun(string funname)
        {
            _funname = funname;
        }
        public void AddParam(TemplateExpress exp)
        {
            _params.AddLast(exp);
        }
        public TAVar Calculate(ITemplateContext context, object scope)
        {
            object[] parameters = MakeParameters(context);
            if (scope == null)
            {
                MethodInfo m = _m;
                if (m == null)
                {
                    m = TemplateApp.Instance.GetSystemMethod(_funname);
                    if (m == null)
                    {
                        throw new Exception(string.Format("{0}方法不存在", _funname));
                    }
                    Interlocked.Exchange(ref _m, m);
                }
                object rt;
                if(Invoke(context, m, parameters, scope, out rt))
                {
                    return new TAVar(rt, VarType.Var);
                }
                throw new Exception(string.Format("{0}方法不存在2", _funname));
            }
            else
            {
                object rt;
                MethodInfo m = _m;
                if (m == null)
                {
                   rt = InvokeMore(context, parameters, scope);
                }
                else
                {
                    if (!Invoke(context, m, parameters, scope, out rt))
                    {
                        rt = InvokeMore(context, parameters, scope);
                    }
                }
                return new TAVar(rt, VarType.Var);
            }
        }
        /// <summary>
        /// 重载函数的执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private object InvokeMore(ITemplateContext context, object[] parameters, object scope)
        {
            Type nextType = scope.GetType();
            MethodInfo[] methods = nextType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo m in methods)
            {
                if (m.Name.Equals(_funname))
                {
                    object rt;
                    if (Invoke(context, m, parameters, scope, out rt))
                    {
                        Interlocked.Exchange(ref _m, m);
                        return rt;
                    }
                }
            }
            throw new Exception(string.Format("{0}方法不存在", _funname));
        }
        private object[] MakeParameters(ITemplateContext context)
        {
            LinkedListNode<TemplateExpress> node = _params.First;
            object[] parameters = new object[_params.Count];
            int i = 0;
            while (node != null)
            {
                parameters[i] = node.Value.Calculate(context).Value;
                node = node.Next;
                i++;
            }
            return parameters;
        }
        private bool Invoke(ITemplateContext context, MethodInfo method, object[] parameters, object scope, out object rt)
        {
            rt = null;
            ParameterInfo[] paramInfoArr = method.GetParameters();
            if (parameters.Length > paramInfoArr.Length)
            {
                return false;
            }
            object[] newparameters = new object[paramInfoArr.Length];
            int endi = paramInfoArr.Length;
            if (newparameters.Length > 0)
            {
                int lasti = paramInfoArr.Length - 1;
                ParameterInfo info = paramInfoArr[lasti];
                object[] attrs = info.GetCustomAttributes(typeof(ParamArrayAttribute), false);
                if (attrs.Length > 0)
                {
                    List<object> tmp = new List<object>();
                    Type elmt = info.ParameterType.GetElementType();
                    for (int i = lasti; i < parameters.Length; i++)
                    {
                        object obj = parameters[i];
                        if (!elmt.IsInterface && !elmt.IsClass)
                        {
                            if (!TAConverter.Instance.TryConvert(obj, info.ParameterType.GetElementType(), out obj))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!elmt.IsAssignableFrom(obj.GetType()))
                            {
                                return false;
                            }
                        }
                        tmp.Add(obj);
                    }
                    newparameters[lasti] = tmp.ToArray();
                    endi = lasti;
                }
            }
            for (int i = 0; i < endi; i++)
            {
                ParameterInfo info = paramInfoArr[i];
                object obj;
                if (i < parameters.Length)
                {
                    obj = parameters[i];
                    if (!info.ParameterType.IsInterface && !info.ParameterType.IsClass)
                    {
                        if (!TAConverter.Instance.TryConvert(obj, info.ParameterType, out obj))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (info.DefaultValue != DBNull.Value)
                    {
                        obj = info.DefaultValue;
                    }
                    else
                    {
                        return false;
                    }
                }
                newparameters[i] = obj;
            }
            rt = method.Invoke(scope, newparameters);
            return true;
        }

        public void Assign(ITemplateContext context, object scope, object result)
        {
            throw new Exception("函数不可以赋值");
        }
    }
}
