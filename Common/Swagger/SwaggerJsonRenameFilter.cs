using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Common.Swagger
{
    public class SwaggerJsonRenameFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var props = context.Type.GetProperties();
            foreach (var prop in props)
            {
                OpenApiSchema outval;
                if (schema.Properties.TryGetValue(prop.Name, out outval))
                {
                    var excludeAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>(true);
                    if (excludeAttr != null && excludeAttr.Name != null)
                    {
                        schema.Properties.Remove(prop.Name);
                        schema.Properties.Add(excludeAttr.Name, outval);
                    }
                }

            }
        }
    }
}
