using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;

namespace TemplateAction.NetCore
{
    /// <summary>
    /// 验证每个Action的输入参数
    /// </summary>
    public abstract class TANetController : TABaseController
    {
        public override async Task<IResult> CallAction(TAAction ac, object[] parameters)
        {
            TANetNodeValidExt ext = IntentAction.ActionNode.GetExtra<TANetNodeValidExt>();
            if (ext != null)
            {
                var paramInfos = ac.ActionNode.Method.GetParameters();
                bool isValid = true;
                List<ValidationResult> validationResults = new List<ValidationResult>();
                for (var i = 0; i < parameters.Length; i++)
                {
                    var value = parameters[i];
                    var valueType = value.GetType();
                    var paramInfo = paramInfos[i];
                    ValidationAttribute[] attrs = (ValidationAttribute[])paramInfo.GetCustomAttributes<ValidationAttribute>();
                    if (attrs.Length > 0)
                    {
                        var context = new ValidationContext(parameters);
                        context.MemberName = paramInfo.Name;
                        isValid &= Validator.TryValidateValue(value, context, validationResults, attrs);
                    }
                    else if (!valueType.IsValueType && valueType != typeof(string))
                    {
                        if (value is IList list)
                        {
                            foreach (object item in list)
                            {
                                isValid &= Validator.TryValidateObject(item, new ValidationContext(item), validationResults, true);
                            }
                        }
                        else
                        {
                            isValid &= Validator.TryValidateObject(value, new ValidationContext(value), validationResults, true);
                        }
                    }
                }
                if (!isValid)
                {
                    if (validationResults != null && validationResults.Count > 0)
                    {

                        return IntentAction.ExceptionFun(ext.Code, new Exception(string.Join(',', validationResults.Select(x => x.ErrorMessage ?? string.Empty))));
                    }
                    else
                    {
                        return IntentAction.ExceptionFun(ext.Code, new Exception("未知错误"));
                    }
                }
            }

            return await base.CallAction(ac, parameters);
        }
    }
}
