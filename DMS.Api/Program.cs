using System.Text.Json.Serialization;
using DMS.Application.Contracts;
using DMS.Application.Services;
using DMS.Infrastructure.Analysis;
using DMS.Infrastructure.Readers;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application
builder.Services.AddScoped<IDocumentValidationService, DocumentValidationService>();

// Readers
builder.Services.AddScoped<TextDocumentReader>();
builder.Services.AddScoped<ExcelDocumentReader>();
builder.Services.AddScoped<PdfDocumentReader>();

builder.Services.AddScoped<IDocumentReader, CompositeDocumentReader>();

// Analyzer
builder.Services.AddScoped<IDocumentAnalyzer, RuleBasedDocumentAnalyzer>();

var app = builder.Build();

// Swagger configuration
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DMS API V1");

    // Abre Swagger na raiz:
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();