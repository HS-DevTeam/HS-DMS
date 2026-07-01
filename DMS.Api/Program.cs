using Asp.Versioning;
using System.Text.Json.Serialization;
using DMS.Application.Contracts;
using DMS.Application.Services;
using DMS.Infrastructure.Analysis;
using DMS.Infrastructure.Readers;
using DMS.Infrastructure.Services;
using DMS.Api.Configs;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using DMS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services
    .AddApiVersioning(options =>
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

builder.Services.AddTransient<
    IConfigureOptions<SwaggerGenOptions>,
    ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile =
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath =
        Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath, true);
});

//
// Application
//
builder.Services.AddScoped<
    IDocumentValidationService,
    DocumentValidationService>();

builder.Services.AddScoped<
    IDocumentProcessingPipeline,
    DocumentProcessingPipeline>();

builder.Services.AddScoped<
    IDocumentAnalyzer,
    RuleBasedDocumentAnalyzer>();

builder.Services.AddScoped<
    IDocumentReader,
    CompositeDocumentReader>();

//
// Infrastructure
//
builder.Services.AddScoped<
    IOcrService,
    TesseractCliOcrService>();

//
// Readers
//
builder.Services.AddScoped<TextDocumentReader>();
builder.Services.AddScoped<ExcelDocumentReader>();
builder.Services.AddScoped<PdfDocumentReader>();

//
// Repositories
//
// IMPLEMENTAR NA INFRASTRUCTURE

 builder.Services.AddScoped< IDocumentTypeRepository, DocumentTypeRepository>();

var app = builder.Build();

app.UseSwagger();

var provider =
    app.Services.GetRequiredService<
        Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider>();

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