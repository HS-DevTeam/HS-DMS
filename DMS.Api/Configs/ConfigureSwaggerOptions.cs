using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DMS.Api.Configs;

public sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // Cria um documento do Swagger para cada versão de API descoberta pelo ecossistema
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForVersion(
        ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = "HS-DMS API",
            Version = description.ApiVersion.ToString(),
            Description =
                """
                Sistema de validação documental da Harmonia Seguros.

                Funcionalidades:
                - Upload de documentos
                - Leitura de PDF, Excel e Texto
                - OCR para documentos digitalizados
                - Deteção automática do tipo documental
                - Classificação baseada em keywords
                - Tipos documentais globais e por tenant
                - Validação documental com score de confiança
                - Extração de metadados (futuro)
                """,
            Contact = new OpenApiContact
            {
                Name = "HS Developer Team"
            }
        };

        if (description.IsDeprecated)
        {
            info.Description +=
                "\n\n⚠️ Esta versão foi descontinuada (Deprecated).";
        }

        return info;
    }
}