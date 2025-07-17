using System.Diagnostics;
using Common.Entities;
using LatexCompiler.Repository.Interfaces;

namespace LatexCompiler.Repository;

public class LatexCompilingRepository : ILatexCompilingRepository
{
    private readonly ILogger<LatexCompilingRepository> _logger;

    public LatexCompilingRepository(ILogger<LatexCompilingRepository> logger)
    {
        _logger = logger;
    }

    public async Task<Stream> CompileAsync(ExtractedLatexProject project, CancellationToken token)
    {
        try
        {
            CleanBuildArtifacts(project.ProjectDirectory);

            var psi = new ProcessStartInfo
            {
                FileName = "latexmk",
                Arguments = $"-synctex=1 -interaction=nonstopmode -file-line-error -pdf -f -g -outdir=\"{project.ProjectDirectory}\" \"{Path.GetFileName(project.MainTexPath)}\"",
                WorkingDirectory = project.ProjectDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            await proc.WaitForExitAsync(token);

            var output = await proc.StandardOutput.ReadToEndAsync();
            var error = await proc.StandardError.ReadToEndAsync();
            _logger.LogDebug($"LaTeX Output:\n{output}");
            _logger.LogDebug($"LaTeX Errors:\n{error}");

            var pdfPath = Path.Combine(project.ProjectDirectory, Path.GetFileNameWithoutExtension(project.MainTexPath) + ".pdf");
            
            if (!File.Exists(pdfPath))
            {
                throw new Exception($"PDF generation failed");
            }

            return new FileStream(pdfPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LaTeX compilation failed");
            throw new Exception($"LaTeX compilation failed: {ex.Message}", ex);
        }
    }

    private void CleanBuildArtifacts(string projectDir)
    {
        var patterns = new[] { "*.aux", "*.bbl", "*.blg", "*.toc", "*.log", "*.out", "*.fls", "*.fdb_latexmk" };
        foreach (var pattern in patterns)
        {
            foreach (var file in Directory.GetFiles(projectDir, pattern))
            {
                try { File.Delete(file); }
                catch { /* Ignore deletion errors */ }
            }
        }
    }
}