using Common.DTO;

namespace EptaCombine.HttpService.Interfaces;

public interface ILatexCompilingHttpService
{
    Task<LatexProjectDTO> UploadAsync(long userId, Stream zipStream, ISession session, CancellationToken token);
    
    Task<string> GetMainTexContentAsync(ISession session, CancellationToken token);
    
    Task<string> GetMainBibContentAsync(ISession session, CancellationToken token);
    
    Task UpdateProjectAsync(ISession session, string texContent, string bibContent, CancellationToken token);
    
    Task<Stream> CompileAsync(ISession session, CancellationToken token);
    
    Task CleanupAsync(ISession session, CancellationToken token);
}