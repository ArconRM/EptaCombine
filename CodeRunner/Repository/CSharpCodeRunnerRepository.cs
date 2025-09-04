using System.Diagnostics;
using System.Text;
using CodeRunner.Entities;
using CodeRunner.Repository.Interfaces;
using Common.DTO;

namespace CodeRunner.Repository;

public class CSharpCodeRunnerRepository : BaseCodeRunnerRepository
{
    public CSharpCodeRunnerRepository(ILogger<BaseCodeRunnerRepository> logger) : base(logger) { }

    protected override LanguageConfig GetLanguageConfig()
    {
        return new CSharpLanguageConfig();
    }
}