using System.Diagnostics;
using System.IO.Compression;
using Common.Entities;
using LatexCompiler.Entities;
using LatexCompiler.Options;
using LatexCompiler.Repository.Interfaces;
using Microsoft.Extensions.Options;

namespace LatexCompiler.Repository;

public class LatexCompilingRepository : ILatexCompilingRepository
{
    private readonly string _root;

    private readonly ILogger<LatexCompilingRepository> _logger;

    public LatexCompilingRepository(IOptions<CompilerSettings> options, ILogger<LatexCompilingRepository> logger)
    {
        _root = options.Value.DataDirectory;
        _logger = logger;
    }

    public LatexProject SaveProjectFromZip(long userId, Stream zipStream)
    {
        var project = new LatexProject
        {
            Uuid = Guid.NewGuid(),
            UserId = userId
        };

        var tempDir = Path.Combine(_root, project.Uuid.ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true))
            {
                archive.ExtractToDirectory(tempDir);
            }

            var macosDir = Path.Combine(tempDir, "__MACOSX");
            if (Directory.Exists(macosDir))
                Directory.Delete(macosDir, true);

            var texFiles = Directory.GetFiles(tempDir, "*.tex", SearchOption.AllDirectories);
            if (!texFiles.Any())
                throw new InvalidOperationException("No .tex files found in archive");
            var mainTexFile = texFiles.FirstOrDefault(f =>
                Path.GetFileName(f).Equals("main.tex", StringComparison.OrdinalIgnoreCase)) ?? texFiles.First();
            var mainTexName = Path.GetFileName(mainTexFile);

            var workDir = Path.GetDirectoryName(mainTexFile);

            var bibFiles = Directory.GetFiles(workDir, "*.bib", SearchOption.AllDirectories);
            if (!bibFiles.Any())
                throw new InvalidOperationException("No bib files found in archive");
            var mainBibName = Path.GetFileName(bibFiles.FirstOrDefault(f =>
                Path.GetFileName(f).Equals("thesis.bib", StringComparison.OrdinalIgnoreCase)) ?? bibFiles.First());

            project.ProjectDirectory = workDir;
            project.MainTexName = mainTexName;
            project.MainBibName = mainBibName;

            return project;
        }
        catch
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            throw;
        }
    }

    public async Task<string> GetMainTexContentAsync(LatexProject project, CancellationToken token)
    {
        var mainTexContent = await File.ReadAllTextAsync(project.MainTexPath, token);
        return mainTexContent;
    }

    public async Task<string> GetMainBibContentAsync(LatexProject project, CancellationToken token)
    {
        var mainBibContent = await File.ReadAllTextAsync(project.MainBibPath, token);
        return mainBibContent;
    }

    public async Task UpdateMainTexAsync(LatexProject project, string content, CancellationToken token)
    {
        await File.WriteAllTextAsync(project.MainTexPath, content, token);
    }

    public async Task UpdateMainBibAsync(LatexProject project, string content, CancellationToken token)
    {
        await File.WriteAllTextAsync(project.MainBibPath, content, token);
    }

    public async Task<Stream> CompileAsync(LatexProject project, CancellationToken token)
    {
        try
        {
            CleanBuildArtifacts(project.ProjectDirectory);

            var psi = new ProcessStartInfo
            {
                FileName = "latexmk",
                Arguments = $"-synctex=1 " +
                            $"-interaction=nonstopmode " +
                            $"-file-line-error " +
                            $"-pdf " +
                            $"-f " +
                            $"-g " +
                            $"-outdir=\"{project.ProjectDirectory}\" \"{Path.GetFileName(project.MainTexPath)}\"",
                WorkingDirectory = project.ProjectDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            await proc.WaitForExitAsync(token);

            var output = await proc.StandardOutput.ReadToEndAsync(token);
            _logger.LogInformation(output);
            
            var error = await proc.StandardError.ReadToEndAsync(token);
            if (!string.IsNullOrEmpty(error))
                _logger.LogError(error);

            var pdfPath = Path.Combine(project.ProjectDirectory,
                Path.GetFileNameWithoutExtension(project.MainTexPath) + ".pdf");

            if (!File.Exists(pdfPath))
            {
                throw new Exception($"PDF generation failed");
            }

            return new FileStream(pdfPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                FileOptions.DeleteOnClose);
        }
        catch (Exception ex)
        {
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
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }

    public void Delete(LatexProject project)
    {
        var dir = new DirectoryInfo(project.ProjectDirectory);

        while (dir != null && !Equals(dir.Name, project.Uuid.ToString()))
        {
            dir = dir.Parent;
        }

        if (dir != null && Directory.Exists(dir.FullName))
            Directory.Delete(dir.FullName, true);
        else if (Directory.Exists(project.ProjectDirectory))
            Directory.Delete(project.ProjectDirectory, true);
    }
}