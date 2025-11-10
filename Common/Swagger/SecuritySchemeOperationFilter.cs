using Common.Share;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Swagger
{
    public class SecuritySchemeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context != null && operation != null)
            {
                string id = "";
                if (typeof(ILoginController).IsAssignableFrom(context.MethodInfo.DeclaringType))
                {
                    id = "系统Api令牌";
                }
                else if (typeof(IDeveloperController).IsAssignableFrom(context.MethodInfo.DeclaringType))
                {
                    id = "开发者Api令牌";
                }

                if (!string.IsNullOrEmpty(id))
                {
                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement {
                            {
                                new OpenApiSecurityScheme
                                {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = id
                                }
                                },
                                new string[] { }
                            }
                        }
                    };
                }



            }
        }
    }
}
