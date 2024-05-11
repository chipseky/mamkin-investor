using Chipseky.MamkinInvestor.WebApi.Infrastructure;
using Chipseky.MamkinInvestor.WebApi.Queries;
using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;

namespace Chipseky.MamkinInvestor.WebApi.Extensions;

public static class OpenApiServiceCollectionExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddOpenApiDocument((settings, _) =>
        {
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