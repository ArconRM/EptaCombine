using System.IO.Compression;
using Common.Entities;
using LatexCompiler.Entities;
using LatexCompiler.Repository.Interfaces;
using LatexCompiler.Service.Interfaces;

namespace LatexCompiler.Service;

public class LatexCompilingService : ILatexCompilingService
{
    private readonly ILatexCompilingRepository _latexCompilingRepository;
    private readonly ILatexProjectRepository _latexProjectRepository;

    public LatexCompilingService(
        ILatexCompilingRepository latexCompilingRepository,
        ILatexProjectRepository latexProjectRepository)
    {
        _latexCompilingRepository = latexCompilingRepository;
        _latexProjectRepository = latexProjectRepository;
    }

    public async Task<IEnumerable<LatexProject>> GetUserProjectsAsync(long userId, CancellationToken token)
    {
        return await _latexProjectRepository.GetUserProjectsAsync(userId, token);
    }

    public async Task<LatexProject> UploadAsync(long userId, Stream zipStream, CancellationToken token)
    {
        var project = _latexCompilingRepository.SaveProjectFromZip(userId, zipStream);
        await _latexProjectRepository.CreateAsync(project, token);
        
        return project;
    }

    public async Task<string> GetMainTexContentAsync(Guid projectUuid, CancellationToken token)
    {
        var project = await GetProjectAsync(projectUuid, token);

        return await _latexCompilingRepository.GetMainTexContentAsync(project, token);
    }

    public async Task<string> GetMainBibContentAsync(Guid projectUuid, CancellationToken token)
    {
        var project = await GetProjectAsync(projectUuid, token);
        
        return await _latexCompilingRepository.GetMainBibContentAsync(project, token);
    }

    public async Task UpdateProject(Guid projectUuid, string texContent, string bibContent, CancellationToken token)
    {
        var project = await GetProjectAsync(projectUuid, token);
        await _latexCompilingRepository.UpdateMainTexAsync(project, texContent, token);
        await _latexCompilingRepository.UpdateMainBibAsync(project, bibContent, token);
    }
    
    public async Task<Stream> CompileAsync(Guid projectUuid, CancellationToken token)
    {
        var project = await GetProjectAsync(projectUuid, token);
        
        return await _latexCompilingRepository.CompileAsync(project, token);
    }

    public async Task CleanupAsync(Guid projectUuid, CancellationToken token)
    {
        var project = await GetProjectAsync(projectUuid, token);
        
        await _latexProjectRepository.DeleteAsync(projectUuid, token);
        _latexCompilingRepository.Delete(project);
    }

    private async Task<LatexProject> GetProjectAsync(Guid projectUuid, CancellationToken token)
    {
        LatexProject project;
        try
        {
            project = await _latexProjectRepository.GetAsync(projectUuid, token);
        }
        catch
        {
            throw new KeyNotFoundException($"Project with Uuid {projectUuid} was not found");
        }

        return project;
    }
}