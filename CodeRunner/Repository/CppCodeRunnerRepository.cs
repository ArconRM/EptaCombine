using CodeRunner.Entities;

namespace CodeRunner.Repository;

public class CppCodeRunnerRepository: BaseCodeRunnerRepository
{

    public CppCodeRunnerRepository(ILogger<BaseCodeRunnerRepository> logger) : base(logger) { }

    protected override LanguageConfig GetLanguageConfig() => new CppLanguageConfig();
}