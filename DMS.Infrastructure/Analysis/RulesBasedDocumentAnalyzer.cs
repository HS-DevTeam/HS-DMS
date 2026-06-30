using System.Globalization;
using System.Text;
using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;

namespace DMS.Infrastructure.Analysis;

public sealed class RuleBasedDocumentAnalyzer : IDocumentAnalyzer
{
    public Task<DocAnalysis> AnalyzeAsync(
        DocumentReadResult input,
        CancellationToken cancellationToken = default)
    {
        // 1. Normaliza o texto removendo acentos de forma robusta
        var content = RemoveAccents(input.Text.ToLowerInvariant());

        var scores = Enum.GetValues<DocType>()
            .Where(t => t != DocType.Unknown)
            .Select(t => new
            {
                Type = t,
                Score = GetScore(t, content)
            })
            .OrderByDescending(x => x.Score)
            .ToList();

        var best = scores.First();

        // Se a pontuação máxima for 0, garante que o tipo retorna Unknown
        var detectedType = best.Score > 0 ? best.Type : DocType.Unknown;

        return Task.FromResult(new DocAnalysis
        {
            DocumentType = detectedType,
            Confidence = best.Score / 100m
        });
    }

    private static int GetScore(DocType type, string content)
    {
        // NOTA: Todas as palavras-chave aqui DEVEM estar em minúsculas e SEM acentos/cedilhas
        return type switch
        {
            DocType.EmployeeList => CalculateScore(
                content,
                new Dictionary<string, int>
                {
                    ["nome"] = 40,
                    ["cargo"] = 30,
                    ["salario"] = 30
                }),

            DocType.VehicleList => CalculateScore(
                content,
                new Dictionary<string, int>
                {
                    ["matricula"] = 40,
                    ["marca"] = 30,
                    ["modelo"] = 30
                }),

            DocType.CommercialCertificate => CalculateScore(
                content,
                new Dictionary<string, int>
                {
                    ["certidao"] = 35,
                    ["certidao comercial"] = 35,
                    ["certificacao comercial"] = 35,
                    ["conservatoria"] = 20,
                    ["registo comercial"] = 20,
                    ["registos comerciais"] = 20,
                    ["matricula"] = 15,
                    ["matricula comercial"] = 15,
                    ["natureza juridica"] = 15,
                    ["sociedade por quotas"] = 15,
                    ["limitada"] = 15,
                    ["lda"] = 15,
                    ["capital social"] = 10,
                    ["nif"] = 5,
                    ["denominacao social"] = 5
                }),

            _ => 0
        };
    }

    private static int CalculateScore(string content, IDictionary<string, int> keywords)
    {
        var score = 0;

        foreach (var keyword in keywords)
        {
            if (content.Contains(keyword.Key))
            {
                score += keyword.Value;
            }
        }

        return Math.Min(score, 100);
    }

    /// <summary>
    /// Remove acentos de forma eficiente usando FormD (Separation of diacritics)
    /// </summary>
    private static string RemoveAccents(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        // Substituição especial para o 'ç' que por vezes não decompõe isolado em algumas normalizações
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC).Replace("ç", "c");
    }
}