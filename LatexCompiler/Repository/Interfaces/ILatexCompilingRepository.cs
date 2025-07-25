using Common.Entities;
using LatexCompiler.Entities;

namespace LatexCompiler.Repository.Interfaces;

public interface ILatexCompilingRepository
{
    LatexProject SaveProjectFromZip(Stream zipStream);
    
    Task<string> GetMainTexContentAsync(LatexProject project, CancellationToken token);
    
    Task<string> GetMainBibContentAsync(LatexProject project, CancellationToken token);
    
    Task UpdateMainTexAsync(LatexProject project, string content, CancellationToken token);
    
    Task UpdateMainBibAsync(LatexProject project, string content, CancellationToken token);
    
    Task<Stream> CompileAsync(LatexProject project, CancellationToken token);
    
    void Delete(LatexProject project);
}