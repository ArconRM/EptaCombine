using Common.Entities;
using LatexCompiler.Entities;

namespace LatexCompiler.Service.Interfaces;

public interface ILatexCompilingService
{
    Task<LatexProject> UploadAsync(Stream zipStream, CancellationToken token);
    
    Task<string> GetMainTexContentAsync(Guid projectUuid, CancellationToken token);
    
    Task UpdateMainTexAsync(Guid projectUuid, string content, CancellationToken token);
    
    Task<Stream> CompileAsync(Guid projectUuid, CancellationToken token);
    
    Task CleanupAsync(Guid projectUuid, CancellationToken token);
}