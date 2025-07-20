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

    public LatexCompilingRepository(IOptions<CompilerSettings> options)
    {
        _root = options.Value.FullTempPath;
    }
    
    public LatexProject SaveProjectFromZip(Stream zipStream)
    {
        var project = new LatexProject
        {
            Uuid = Guid.NewGuid()
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

            var mainTex = texFiles.FirstOrDefault(f => 
                Path.GetFileName(f).Equals("main.tex", StringComparison.OrdinalIgnoreCase)) ?? texFiles.First();

            var workDir = Path.GetDirectoryName(mainTex);

            project.ProjectDirectoryName = workDir;

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

    public async Task UpdateMainTexAsync(LatexProject project, string content, CancellationToken token)
    {
        await File.WriteAllTextAsync(project.MainTexPath, content, token);
    }

    public async Task<Stream> CompileAsync(LatexProject project, CancellationToken token)
    {try
        {
            CleanBuildArtifacts(project.ProjectDirectoryName);

            var psi = new ProcessStartInfo
            {
                FileName = "latexmk",
                Arguments = $"-synctex=1 " +
                            $"-interaction=nonstopmode " +
                            $"-file-line-error " +
                            $"-pdf " +
                            $"-f " +
                            $"-g " +
                            $"-outdir=\"{project.ProjectDirectoryName}\" \"{Path.GetFileName(project.MainTexPath)}\"",
                WorkingDirectory = project.ProjectDirectoryName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            await proc.WaitForExitAsync(token);

            var output = await proc.StandardOutput.ReadToEndAsync(token);
            var error = await proc.StandardError.ReadToEndAsync(token);

            var pdfPath = Path.Combine(project.ProjectDirectoryName, Path.GetFileNameWithoutExtension(project.MainTexPath) + ".pdf");
            
            if (!File.Exists(pdfPath))
            {
                throw new Exception($"PDF generation failed");
            }

            return new FileStream(pdfPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);
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
        if (Directory.Exists(project.ProjectDirectoryName))
            Directory.Delete(project.ProjectDirectoryName, true);
    }
}