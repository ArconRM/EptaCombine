using Common.DTO;

namespace CodeRunner.Repository.Interfaces;

public interface ICSharpCodeRunnerRepository
{
    Task<CodeExecutionResult> RunCodeAsync(string code, CancellationToken token);
}