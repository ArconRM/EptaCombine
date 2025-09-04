using Common.DTO;

namespace CodeRunner.Repository.Interfaces;

public interface ICodeRunnerRepository
{
    Task<CodeExecutionResult> RunCodeAsync(string code, CancellationToken token);
}