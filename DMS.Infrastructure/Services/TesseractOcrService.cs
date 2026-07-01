using System.Diagnostics;
using DMS.Application.Contracts;

namespace DMS.Infrastructure.Services;

public sealed class TesseractCliOcrService : IOcrService
{
    public async Task<string> ExtractTextAsync(
        byte[] fileBytes,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var tempFile = Path.GetTempFileName();

        await File.WriteAllBytesAsync(
            tempFile,
            fileBytes,
            cancellationToken);

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

            using var process = Process.Start(psi)
                ?? throw new InvalidOperationException(
                    "Unable to start Tesseract process.");

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync(cancellationToken);

            var output = await outputTask;
            var error = await errorTask;

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"Tesseract exited with code {process.ExitCode}: {error}");
            }

            return output;
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}