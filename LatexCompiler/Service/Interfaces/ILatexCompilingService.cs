namespace LatexCompiler.Service.Interfaces;

public interface ILatexCompilingService
{
    Task<Stream> CompileAsync(Stream inputZipStream, CancellationToken token);
}