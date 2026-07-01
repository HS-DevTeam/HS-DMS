using System.Diagnostics;
using DMS.Application.Contracts;

namespace DMS.Infrastructure.Services;

public sealed class TesseractCliOcrService : IOcrService
{
    public string ExtractText(byte[] fileBytes, string contentType)
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllBytes(tempFile, fileBytes);

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "tesseract",
                Arguments = $"{tempFile} stdout -l por",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)!;
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}