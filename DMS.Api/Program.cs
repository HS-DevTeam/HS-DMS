using Asp.Versioning; // <-- Adicionar este
using System.Text.Json.Serialization;
using DMS.Application.Contracts;
using DMS.Application.Services;
using DMS.Infrastructure.Analysis;
using DMS.Infrastructure.Readers;
using DMS.Api.Configs; // Namespace onde criou o ConfigureSwaggerOptions
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// --- NOVO: Configuração de Versionamento de API ---
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Retorna as versões suportadas no Header da resposta
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Formata como 'v1', 'v2', 'v1.1'
    options.SubstituteApiVersionInUrl = true; // Substitui o {version:apiVersion} na rota automaticamente
});

builder.Services.AddEndpointsApiExplorer();

// --- NOVO: Configuração Dinâmica do Swagger Gen ---
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    // Adiciona apenas os comentários XML genéricos (as versões são resolvidas pelo ConfigureSwaggerOptions)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, true);
});

// --- DI de Aplicação e Infraestrutura mantêm-se iguais ---
builder.Services.AddScoped<IDocumentValidationService, DocumentValidationService>();
builder.Services.AddScoped<TextDocumentReader>();
builder.Services.AddScoped<ExcelDocumentReader>();
builder.Services.AddScoped(sp => new OcrDocumentReader(
    tessDataPath: Path.Combine(AppContext.BaseDirectory, "tessdata"),
    language: "por"
));
builder.Services.AddScoped<PdfDocumentReader>();
builder.Services.AddScoped<IDocumentReader, CompositeDocumentReader>();
builder.Services.AddScoped<IDocumentAnalyzer, RuleBasedDocumentAnalyzer>();

var app = builder.Build();

app.UseSwagger();

// --- NOVO: Atualização do UI para listar as versões dinamicamente ---
var provider = app.Services.GetRequiredService<Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider>();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "HS-DMS API";
    
    // Gera um endpoint no dropdown do Swagger para cada versão encontrada
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json", 
            $"HS-DMS API {description.GroupName.ToUpperInvariant()}");
    }
    
    options.RoutePrefix = "swagger";
});

app.MapControllers();
app.Run();