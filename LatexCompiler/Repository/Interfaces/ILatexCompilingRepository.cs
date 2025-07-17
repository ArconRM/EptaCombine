using Common.Entities;

namespace LatexCompiler.Repository.Interfaces;

public interface ILatexCompilingRepository
{
    Task<Stream> CompileAsync(ExtractedLatexProject project, CancellationToken token);
}