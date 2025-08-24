using Common.Entities;
using LatexCompiler.Entities;

namespace LatexCompiler.Service.Interfaces;

public interface ILatexCompilingService
{
    Task<IEnumerable<LatexProject>> GetUserProjectsAsync(long userId, CancellationToken token);
    
    Task<LatexProject> CreateProjectFromTemplateAsync(long userId, CancellationToken token);
    
    Task<LatexProject> CreateProjectFromZipAsync(long userId, Stream zipStream, CancellationToken token);
    
    Task<string> GetMainTexContentAsync(Guid projectUuid, CancellationToken token);
    
    Task<string> GetMainBibContentAsync(Guid projectUuid, CancellationToken token);
    
    Task UpdateProject(Guid projectUuid, string texContent, string bibContent, CancellationToken token);
    
    Task<Stream> CompileAsync(Guid projectUuid, CancellationToken token);
    
    Task CleanupAsync(Guid projectUuid, CancellationToken token);
}