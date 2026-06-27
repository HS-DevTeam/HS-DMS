using DMS.Application.Contracts;
using DMS.Application.Services;
using DMS.Domain.Documents;

namespace DMS.Infrastructure.Analysis;

public sealed class RuleBasedDocumentAnalyzer : IDocumentAnalyzer
{
    public Task<DocAnalysis> AnalyzeAsync(
        DocumentReadResult input,
        CancellationToken cancellationToken = default)
    {
        var content = Normalize(input.Text);

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

        return Task.FromResult(new DocAnalysis
        {
            DocumentType = best.Type,
            Confidence = best.Score / 100m
        });
    }

    private static int GetScore(
        DocType type,
        string content)
    {
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

    private static int CalculateScore(
        string content,
        IDictionary<string, int> keywords)
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

    private static string Normalize(string text)
    {
        text = text.ToLowerInvariant();

        return text
            .Replace("á", "a")
            .Replace("à", "a")
            .Replace("â", "a")
            .Replace("ã", "a")
            .Replace("é", "e")
            .Replace("ê", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ô", "o")
            .Replace("õ", "o")
            .Replace("ú", "u")
            .Replace("ç", "c");
    }
}