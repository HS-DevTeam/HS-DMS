using Asp.Versioning;
using System.Text.Json.Serialization;
using DMS.Application.Contracts;
using DMS.Application.Services;
using DMS.Infrastructure.Analysis;
using DMS.Infrastructure.Readers;
using DMS.Api.Configs;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using DMS.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, true);
    }
});

// =========================
// Dependency Injection
// =========================

builder.Services.AddScoped<IDocumentValidationService, DocumentValidationService>();

builder.Services.AddScoped<TextDocumentReader>();
builder.Services.AddScoped<ExcelDocumentReader>();
builder.Services.AddScoped<PdfDocumentReader>();
builder.Services.AddScoped<OcrDocumentReader>();

builder.Services.AddScoped<IOcrService, TesseractCliOcrService>();

builder.Services.AddScoped<IDocumentReader, CompositeDocumentReader>();
builder.Services.AddScoped<IDocumentProcessingPipeline, DocumentProcessingPipeline>();
builder.Services.AddScoped<IDocumentAnalyzer, RuleBasedDocumentAnalyzer>();

var app = builder.Build();

// Swagger
app.UseSwagger();

var provider = app.Services.GetRequiredService<Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider>();

app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "HS-DMS API";

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