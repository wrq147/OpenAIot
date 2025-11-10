using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class MvcMiddleware : FilterMiddlewareNode, IFilterMiddleware
    {
        public MvcMiddleware() : base(null) { }

        /// <summary>
        /// 参数绑定
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="pi"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<object> Mapping(TAAction ac, ParameterInfo pi)
        {
            object result = DBNull.Value;
            AbstractMappingAttribute[] attrs = (AbstractMappingAttribute[])pi.GetCustomAttributes<AbstractMappingAttribute>();
            if (attrs.Length == 0)
            {
                LinkedListNode<IParamMapping> node = ac.Context.Application.FirstParamMapping();
                result = await node.Value.Mapping(node.Next, ac, pi.Name, pi.ParameterType);
            }
            else
            {
                foreach (AbstractMappingAttribute att in attrs)
                {
                    result = await att.Mapping(ac, pi.Name, pi.ParameterType);
                }
            }
            if (result == DBNull.Value && pi.DefaultValue != DBNull.Value)
            {
                result = pi.DefaultValue;
            }
            return result;
        }
        public override async Task<IResult> Excute(TAAction ac)
        {
            if (ac.ControllerNode == null) return null;
            if (ac.ActionNode == null) return null;
            ITAContext context = ac.Context;
            Type ct = ac.ControllerNode.ControllerType;
            IController c = null;
            try
            {
                //创建控制器
                c = context.Application.ServiceProvider.CreateScopeService(ac, ct) as IController;
                if (c == null)
                {
                    return null;
                }
                //初始化请求的异常处理
                if (ac.ExceptionFun == null)
                {
                    ac.ExceptionFun = c.Exception;
                }
                //绑定控制器
                c.Bind(ac);

                ParameterInfo[] pinfos = ac.ActionNode.Method.GetParameters();

                //开始遍历参数进行验证
                if (pinfos.Length > 0)
                {
                    List<object> paramlist = new List<object>();
                    for (int i = 0; i < pinfos.Length; i++)
                    {
                        //创建参数映射
                        object mapobj = await Mapping(ac, pinfos[i]);
                        if (mapobj == DBNull.Value)
                        {
                            return new V404Result();
                        }
                        else
                        {
                            paramlist.Add(mapobj);
                        }
                    }
                    return await c.CallAction(ac, paramlist.ToArray());
                }
                else
                {
                    return await c.CallAction(ac, Array.Empty<object>());
                }
            }
            catch (Exception ex)
            {
                if (c == null)
                {
                    return new TextResult(ex.Message);
                }
                if (ex.InnerException != null)
                {
                    return c.Exception(TAUtility.EXCEPTION_CODE, ex.InnerException);
                }
                else
                {
                    return c.Exception(TAUtility.EXCEPTION_CODE, ex);
                }
            }
        }
        public async Task<IResult> Excute(TAAction ac, FilterMiddlewareNode next)
        {
            FilterMiddlewareNode firstNode = this;
            FilterMiddlewareNode lasttNode = null;
            IEnumerable<ActionFilterAttribute> customFilters = ac.ActionNode.Method.GetCustomAttributes<ActionFilterAttribute>();
            foreach (ActionFilterAttribute af in customFilters)
            {
                FilterMiddlewareNode node = new FilterMiddlewareNode(af);
                node.Next = this;
                if (lasttNode == null)
                {
                    lasttNode = node;
                    firstNode = node;
                }
                else
                {
                    lasttNode.Next = node;
                    lasttNode = node;
                }
            }
            return await firstNode.Excute(ac);
        }
    }
}
