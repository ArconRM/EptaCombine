using Common.DTO;

namespace EptaCombine.HttpService.Interfaces;

public interface ILatexCompilingHttpService
{
    Task<LatexProjectDTO> UploadAsync(Stream zipStream, ISession session, CancellationToken token);
    
    Task<string> GetMainTexContentAsync(ISession session, CancellationToken token);
    
    Task UpdateMainTexAsync(ISession session, string content, CancellationToken token);
    
    Task<Stream> CompileAsync(ISession session, CancellationToken token);
    
    Task CleanupAsync(ISession session, CancellationToken token);
}