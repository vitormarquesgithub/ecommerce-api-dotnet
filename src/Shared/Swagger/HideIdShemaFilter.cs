using ECommerce.Api.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class HideIdSchemaFilter : ISchemaFilter {
    public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
        if (context.Type.IsSubclassOf(typeof(AbstractEntity))) {
            if (schema.Properties.ContainsKey("id")) {
                schema.Properties.Remove("id");
            }
        }
    }
}