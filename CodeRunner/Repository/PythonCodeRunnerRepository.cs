using CodeRunner.Entities;

namespace CodeRunner.Repository;

public class PythonCodeRunnerRepository: BaseCodeRunnerRepository
{

    public PythonCodeRunnerRepository(ILogger<BaseCodeRunnerRepository> logger) : base(logger) { }
    
    protected override LanguageConfig GetLanguageConfig() => new PythonLanguageConfig();
}