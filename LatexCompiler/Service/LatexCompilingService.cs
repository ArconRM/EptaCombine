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
    private const string SessionKey = "LatexProjectId";

    public LatexCompilingService(
        ILatexCompilingRepository latexCompilingRepository,
        ILatexProjectRepository latexProjectRepository)
    {
        _latexCompilingRepository = latexCompilingRepository;
        _latexProjectRepository = latexProjectRepository;
    }

    public async Task<LatexProject> UploadAsync(Stream zipStream, ISession session, CancellationToken token)
    {
        var project = _latexCompilingRepository.SaveProjectFromZip(zipStream);
        project.Uuid = Guid.NewGuid();
        
        session.SetString(SessionKey, project.Uuid.ToString());
        await _latexProjectRepository.CreateAsync(project, token);
        
        return project;
    }

    public async Task<string> GetMainTexContentAsync(ISession session, CancellationToken token)
    {
        var projectUuid = Guid.Parse(session.GetString(SessionKey));
        var project = await GetProjectAsync(projectUuid, token);

        return await _latexCompilingRepository.GetMainTexContentAsync(project, token);
    }

    public async Task UpdateMainTexAsync(ISession session, string content, CancellationToken token)
    {
        var projectUuid = Guid.Parse(session.GetString(SessionKey));
        var project = await GetProjectAsync(projectUuid, token);
        
        await _latexCompilingRepository.UpdateMainTexAsync(project, content, token);
    }

    public async Task<Stream> CompileAsync(ISession session, CancellationToken token)
    {
        var projectUuid = Guid.Parse(session.GetString(SessionKey));
        var project = await GetProjectAsync(projectUuid, token);
        
        return await _latexCompilingRepository.CompileAsync(project, token);
    }

    public async Task CleanupAsync(ISession session, CancellationToken token)
    {
        var projectUuid = Guid.Parse(session.GetString(SessionKey));
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