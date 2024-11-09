using Mamkin.In.WebApi.Contracts.Paging;
using Mamkin.In.WebApi.Infrastructure;
using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;

namespace Mamkin.In.WebApi.Extensions;

public static class OpenApiServiceCollectionExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddOpenApiDocument((settings, _) =>
        {
            settings.Title = "mamkin investor api";
            settings.SchemaSettings.TypeMappers.Add(new PagedDataSwaggerMapper());
        });

        return services;
    }

    private class PagedDataSwaggerMapper : ITypeMapper
    {
        public Type MappedType => typeof(IPagedData<>);

        public bool UseReference => true;

        public void GenerateSchema(JsonSchema schema, TypeMapperContext context)
        {
            var resolver = context.JsonSchemaResolver;
            if (!resolver.HasSchema(context.Type, false))
            {
                var generator = context.JsonSchemaGenerator;
                var keyType = context.Type.GenericTypeArguments[0];
                var pagedSchema = generator.Generate(typeof(PagedData<>).MakeGenericType(keyType), resolver);
                foreach (var pagedSchemaActualProperty in pagedSchema.ActualProperties)
                {
                    pagedSchemaActualProperty.Value.IsNullableRaw = false;
                    pagedSchemaActualProperty.Value.IsRequired = true;
                }

                resolver.AddSchema(context.Type, false, pagedSchema);
            }

            schema.Reference = resolver.GetSchema(context.Type, false);
        }
    }
}