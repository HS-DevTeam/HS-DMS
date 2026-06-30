using System.Text.Json.Serialization;
using DMS.Application.Contracts;
using DMS.Application.Services;
using DMS.Infrastructure.Analysis;
using DMS.Infrastructure.Readers;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HS-DMS API",
        Version = "v1",
        Description =
            """
            Sistema de validação documental da Harmonia Seguros.

            Funcionalidades:
            - Upload de documentos
            - Deteção automática do tipo documental
            - Validação baseada em regras
            - OCR (futuro)
            - Extração de metadados (futuro)
            """,
        Contact = new OpenApiContact
        {
            Name = "HS Developer Team"
        }
    });

    var xmlFile =
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath =
        Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath, true);
});

// Application
builder.Services.AddScoped<IDocumentValidationService, DocumentValidationService>();

// Infrastructure
builder.Services.AddScoped<TextDocumentReader>();
builder.Services.AddScoped<ExcelDocumentReader>();
builder.Services.AddScoped<PdfDocumentReader>();

builder.Services.AddScoped<IDocumentReader, CompositeDocumentReader>();
builder.Services.AddScoped<IDocumentAnalyzer, RuleBasedDocumentAnalyzer>();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "HS-DMS API";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "HS-DMS API v1");
    options.RoutePrefix = "swagger";
});

app.MapControllers();

app.Run();