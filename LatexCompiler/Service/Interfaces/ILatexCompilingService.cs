using Common.Entities;
using LatexCompiler.Entities;

namespace LatexCompiler.Service.Interfaces;

public interface ILatexCompilingService
{
    Task<LatexProject> UploadAsync(Stream zipStream, ISession session, CancellationToken token);
    
    Task<string> GetMainTexContentAsync(ISession session, CancellationToken token);
    
    Task UpdateMainTexAsync(ISession session, string content, CancellationToken token);
    
    Task<Stream> CompileAsync(ISession session, CancellationToken token);
    
    Task CleanupAsync(ISession session, CancellationToken token);
}