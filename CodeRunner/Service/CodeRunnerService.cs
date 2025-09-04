using CodeRunner.Repository.Interfaces;
using CodeRunner.Service.Interfaces;
using Common.DTO;
using Common.Entities.Enums;

namespace CodeRunner.Service;

public class CodeRunnerService : ICodeRunnerService
{
    private readonly IServiceProvider _serviceProvider;

    public CodeRunnerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<CodeExecutionResult> RunCodeAsync(string code, ProgramLanguage language, CancellationToken token)
    {
        var runner = _serviceProvider.GetRequiredKeyedService<ICodeRunnerRepository>(language);
        return await runner.RunCodeAsync(code, token);
    }
}