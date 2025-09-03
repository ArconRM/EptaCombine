using Common.DTO;
using Common.Entities.Enums;

namespace CodeRunner.Service.Interfaces;

public interface ICodeRunnerService
{
    Task<CodeExecutionResult> RunCodeAsync(string code, ProgramLanguage language, CancellationToken token);
}