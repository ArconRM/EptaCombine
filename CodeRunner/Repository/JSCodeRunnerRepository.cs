using System.Diagnostics;
using System.Text;
using CodeRunner.Entities;
using CodeRunner.Repository.Interfaces;
using Common.DTO;

namespace CodeRunner.Repository;

public class JSCodeRunnerRepository : BaseCodeRunnerRepository
{
    public JSCodeRunnerRepository(ILogger<BaseCodeRunnerRepository> logger) : base(logger) { }

    protected override LanguageConfig GetLanguageConfig() => new JSLanguageConfig();
}