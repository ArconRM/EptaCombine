using System.IO.Compression;
using Common.Entities;
using LatexCompiler.Repository.Interfaces;
using LatexCompiler.Service.Interfaces;

namespace LatexCompiler.Service;

public class LatexCompilingService : ILatexCompilingService
{
    private readonly ILatexCompilingRepository _repository;
    private readonly ILogger<LatexCompilingService> _logger;

    public LatexCompilingService(ILatexCompilingRepository repository, ILogger<LatexCompilingService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Stream> CompileAsync(Stream inputZipStream, CancellationToken token)
    {
        var project = await ExtractAsync(inputZipStream, token);
        
        try
        {
            return await _repository.CompileAsync(project, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Compilation failed");
            throw;
        }
        finally
        {
            try
            {
                if (Directory.Exists(project.ProjectDirectory))
                    Directory.Delete(project.ProjectDirectory, true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clean up temporary directory");
            }
        }
    }
    
    private async Task<ExtractedLatexProject> ExtractAsync(Stream zipStream, CancellationToken token)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
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

            return new ExtractedLatexProject
            {
                ProjectDirectory = workDir,
                MainTexPath = mainTex
            };
        }
        catch
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            throw;
        }
    }
}