using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;


namespace Midas.API.Filters
{
    public class FiltroCamposBooleanos : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(Infrastructure.Persistence.Entities.Cofrinho) ||
                context.Type == typeof(DTOs.CofrinhoDTO))
            {
                if (schema.Properties.ContainsKey("aplicado"))
                {
                    schema.Properties["aplicado"].Type = "string";
                    schema.Properties["aplicado"].Example = new OpenApiString("F");
                    schema.Properties["aplicado"].Description = "Indica se o cofrinho está aplicado (T/F)";
                    schema.Properties["aplicado"].Pattern = "^[TF]$";
                    schema.Properties["aplicado"].MaxLength = 1;
                }
            }
            if (context.Type == typeof(Infrastructure.Persistence.Entities.Gasto) ||
                context.Type == typeof(DTOs.GastoDTO))
            {
                if (schema.Properties.ContainsKey("fixo"))
                {
                    schema.Properties["fixo"].Type = "string";
                    schema.Properties["fixo"].Example = new OpenApiString("F");
                    schema.Properties["fixo"].Description = "Indica se o gasto é fixo (T/F)";
                    schema.Properties["fixo"].Pattern = "^[TF]$";
                    schema.Properties["fixo"].MaxLength = 1;
                }
            }

            if (context.Type == typeof(Infrastructure.Persistence.Entities.Receita) ||
                context.Type == typeof(DTOs.ReceitaDTO))
            {
                if (schema.Properties.ContainsKey("fixo"))
                {
                    schema.Properties["fixo"].Type = "string";
                    schema.Properties["fixo"].Example = new OpenApiString("F");
                    schema.Properties["fixo"].Description = "Indica se a receita é fixa (T/F)";
                    schema.Properties["fixo"].Pattern = "^[TF]$";
                    schema.Properties["fixo"].MaxLength = 1;
                }
            }
        }
    }
}