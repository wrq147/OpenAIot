using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

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
                    var attributes = prop.GetCustomAttributes(true);
                    var excludeAttr = attributes.OfType<JsonProperty>().FirstOrDefault();
                    if (excludeAttr != null && excludeAttr.PropertyName != null)
                    {
                        schema.Properties.Remove(prop.Name);
                        schema.Properties.Add(excludeAttr.PropertyName, outval);
                    }
                }

            }
        }
    }
}
