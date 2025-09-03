using CodeRunner.Repository.Interfaces;
using CodeRunner.Service.Interfaces;
using Common.DTO;
using Common.Entities.Enums;

namespace CodeRunner.Service;

public class CodeRunnerService : ICodeRunnerService
{
    private readonly ICSharpCodeRunnerRepository _cSharpCodeRunnerRepository;

    public CodeRunnerService(ICSharpCodeRunnerRepository cSharpCodeRunnerRepository)
    {
        _cSharpCodeRunnerRepository = cSharpCodeRunnerRepository;
    }

    public async Task<CodeExecutionResult> RunCodeAsync(string code, ProgramLanguage language, CancellationToken token)
    {
        switch (language)
        {
            case ProgramLanguage.CSharp:
                return await _cSharpCodeRunnerRepository.RunCodeAsync(code, token);

            case ProgramLanguage.Python:
                throw new NotImplementedException();

            default:
                throw new ArgumentOutOfRangeException(nameof(language), language, null);
        }
    }
}