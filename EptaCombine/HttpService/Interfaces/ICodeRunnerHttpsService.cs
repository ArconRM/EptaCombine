using Common.DTO;
using Common.Entities.Enums;

namespace EptaCombine.HttpService.Interfaces;

public interface ICodeRunnerHttpsService
{
    Task<CodeExecutionResult> RunCodeAsync(string code, ProgramLanguage language, CancellationToken token);
}