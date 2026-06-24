using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    /// <summary>
    /// 提供WebApi路由格式的接口
    /// </summary>
    public class TANetWebApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
    {
        private ITAServiceProvider _provider;
        private IModelMetadataProvider _MetadataProvider;
        private ApiDescriptionGroupCollection _cacheApiDescription;
        public TANetWebApiDescriptionGroupCollectionProvider(ITAServiceProvider provider)
        {
            _provider = provider;
            _MetadataProvider = new EmptyModelMetadataProvider();
        }
        /// <summary>
        /// 清缓存
        /// </summary>
        public void ClearCache()
        {
            _cacheApiDescription = null;
        }
        public ApiDescriptionGroupCollection ApiDescriptionGroups
        {
            get
            {
                if (_cacheApiDescription == null)
                {
                    List<ApiDescription> results = new List<ApiDescription>();
                    PluginCollection plgColl = _provider as PluginCollection;
                    PluginObject[] plgArray = plgColl.GetAllPlugin();
                    foreach (PluginObject plg in plgArray)
                    {
                        GenerateApiFromPlugin(plg, results);
                    }
                    var groups = results.GroupBy(d => d.GroupName).Select(g => new ApiDescriptionGroup(g.Key, g.ToArray())).ToArray();
                    _cacheApiDescription = new ApiDescriptionGroupCollection(groups, 1);
                }
                return _cacheApiDescription;
            }
        }
        private void GenerateApiFromPlugin(PluginObject plg, List<ApiDescription> results)
        {
            Dictionary<string, ControllerNode> data = plg.Data.Get<Dictionary<string, ControllerNode>>();
            foreach (var item in data)
            {
                foreach (var itemit in item.Value.Childrens)
                {
                    ActionNode ac = (ActionNode)itemit.Value;
                    GenerateApiDescription(results, item.Value, ac);
                }
            }
        }

        private void GenerateApiDescription(List<ApiDescription> results, ControllerNode controller, ActionNode action)
        {
            ActionDescriptor acdesc = CreateActionDescriptor(controller, action);
            IList<ApiParameterDescription> tApiParameters = GetParameters(acdesc.Parameters);
            ApiExplorerOptions apiExpOpt = _provider.GetService<ApiExplorerOptions>();
            string relativePath = apiExpOpt.RoutePathFun?.Invoke(controller, action);
            if (relativePath == null)
            {
                relativePath = string.Format("{0}/{1}/{2}", controller.PluginName, controller.Key, action.Key);
            }
            if (action.AllowHttpMethod != 0)
            {
                string[] httpMethods = new string[] { "GET", "POST", "PUT", "DELETE" };
                foreach (string curHttpMethod in httpMethods)
                {
                    if (action.JudgeHttpMethod(curHttpMethod))
                    {
                        ApiDescription apiDes = new ApiDescription()
                        {
                            ActionDescriptor = acdesc,
                            GroupName = controller.Descript,
                            HttpMethod = curHttpMethod,
                            RelativePath = relativePath,
                        };

                        var rspattr = action.Method.GetCustomAttribute<TANetWebApiResponseAttribute>();
                        if (rspattr != null)
                        {
                            ApiResponseType rp = new ApiResponseType();
                            rp.IsDefaultResponse = true;
                            rp.ModelMetadata = _MetadataProvider.GetMetadataForType(rspattr.Type);
                            rp.Type = rspattr.Type;
                            rp.StatusCode = rspattr.StatusCode;
                            rp.ApiResponseFormats = new List<ApiResponseFormat>();
                            ApiResponseFormat format = new ApiResponseFormat();
                            format.MediaType = "application/json";
                            var jsonOpts = new JsonSerializerOptions
                            {
                                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                            };
                            format.Formatter = new SystemTextJsonOutputFormatter(jsonOpts);
                            rp.ApiResponseFormats.Add(format);
                            apiDes.SupportedResponseTypes.Add(rp);
                        }
                        else
                        {
                            Type rawReturnType = action.Method.ReturnType;
                            if (action.Method.ReturnType.IsGenericType && action.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                            {
                                if (action.Method.ReturnType.GenericTypeArguments.Length > 0)
                                {
                                    rawReturnType = action.Method.ReturnType.GenericTypeArguments[0];
                                }
                            }
                            if (rawReturnType != typeof(AjaxResult) && typeof(AjaxResult).IsAssignableFrom(rawReturnType))
                            {
                                ApiResponseType rp = new ApiResponseType();
                                rp.IsDefaultResponse = true;
                                rp.ModelMetadata = _MetadataProvider.GetMetadataForType(rawReturnType);
                                rp.Type = rawReturnType;
                                rp.StatusCode = 200;
                                rp.ApiResponseFormats = new List<ApiResponseFormat>();
                                ApiResponseFormat format = new ApiResponseFormat();
                                format.MediaType = "application/json";
                                var jsonOpts = new JsonSerializerOptions
                                {
                                    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                                };
                                format.Formatter = new SystemTextJsonOutputFormatter(jsonOpts);
                                rp.ApiResponseFormats.Add(format);
                                apiDes.SupportedResponseTypes.Add(rp);
                            }
                        }

                        if (curHttpMethod == "GET")
                        {
                            foreach (ApiParameterDescription p in tApiParameters)
                            {
                                if (p.Type.IsValueType || typeof(string).Equals(p.Type))
                                {
                                    p.Source = BindingSource.Query;
                                    apiDes.ParameterDescriptions.Add(p);
                                }
                                else if (p.Type.IsArray)
                                {
                                    p.Source = BindingSource.Query;
                                    apiDes.ParameterDescriptions.Add(p);
                                }
                                else
                                {
                                    PropertyInfo[] ppii = p.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                                    foreach (PropertyInfo pp in ppii)
                                    {
                                        ModelMetadata mmd = _MetadataProvider.GetMetadataForProperty(p.Type, pp.Name);
                                        var parameter = new ApiParameterDescription
                                        {
                                            Name = pp.Name,
                                            Source = BindingSource.Query,
                                            IsRequired = false,
                                            Type = pp.PropertyType,
                                            ModelMetadata = mmd
                                        };
                                        apiDes.ParameterDescriptions.Add(parameter);
                                    }

                                }

                            }

                        }
                        else
                        {
                            foreach (ApiParameterDescription p in tApiParameters)
                            {
                                apiDes.SupportedRequestFormats.Add(new ApiRequestFormat() { Formatter = new SystemTextJsonInputFormatter(new JsonOptions(), _provider.GetService<ILogger<SystemTextJsonInputFormatter>>()), MediaType = "application/json" });
                                apiDes.SupportedRequestFormats.Add(new ApiRequestFormat() { MediaType = "application/x-www-form-urlencoded" });
                                apiDes.SupportedRequestFormats.Add(new ApiRequestFormat() { MediaType = "multipart/form-data" });
                                apiDes.ParameterDescriptions.Add(p);
                            }

                        }

                        results.Add(apiDes);
                    }
                }
            }
        }

        private ActionDescriptor CreateActionDescriptor(ControllerNode controller, ActionNode action)
        {
            var parameterDescriptors = action.Method.GetParameters()
                .Select(CreateParameterDescriptor)
                .ToList();

            var routeValues = new Dictionary<string, string>
            {
                ["controller"] = controller.Key
            };

            return new ControllerActionDescriptor
            {
                AttributeRouteInfo = null,
                ControllerTypeInfo = controller.ControllerType.GetTypeInfo(),
                ControllerName = controller.Key,
                MethodInfo = action.Method,
                Parameters = parameterDescriptors,
                RouteValues = routeValues
            };
        }

        private ParameterDescriptor CreateParameterDescriptor(ParameterInfo parameterInfo)
        {
            return new ControllerParameterDescriptor
            {
                Name = parameterInfo.Name,
                ParameterInfo = parameterInfo,
                ParameterType = parameterInfo.ParameterType
            };
        }


        /// <summary>
        /// 生成Api参数
        /// </summary>
        /// <param name="paramerters"></param>
        /// <returns></returns>
        private IList<ApiParameterDescription> GetParameters(IList<ParameterDescriptor> paramerters)
        {
            List<ApiParameterDescription> apiParamers = new List<ApiParameterDescription>();
            foreach (var actionParameter in paramerters)
            {
                BindingSource bindingSource = BindingSource.Query;
                if (!actionParameter.ParameterType.IsValueType && !typeof(string).Equals(actionParameter.ParameterType))
                {
                    bindingSource = BindingSource.Body;
                }

                bool isRequired = true;
                if (actionParameter is ControllerParameterDescriptor controllerParameter)
                {
                    if (controllerParameter.ParameterInfo.DefaultValue != DBNull.Value)
                    {
                        isRequired = false;
                    }


                    if (controllerParameter.ParameterInfo.IsDefined(typeof(BodyMappingAttribute)))
                    {
                        bindingSource = BindingSource.Body;
                    }
                    else if (controllerParameter.ParameterInfo.IsDefined(typeof(FormMappingAttribute)))
                    {
                        bindingSource = BindingSource.Form;
                    }
                    else if (controllerParameter.ParameterInfo.IsDefined(typeof(QueryMappingAttribute)))
                    {
                        bindingSource = BindingSource.Query;
                    }
                }


                ModelMetadata mmdata = _MetadataProvider.GetMetadataForType(actionParameter.ParameterType);
                var parameter = new ApiParameterDescription
                {
                    Name = actionParameter.Name,
                    Source = bindingSource,
                    IsRequired = isRequired,
                    Type = actionParameter.ParameterType,
                    ParameterDescriptor = actionParameter,
                    ModelMetadata = mmdata
                };
                apiParamers.Add(parameter);
            }

            return apiParamers;
        }
    }
}
