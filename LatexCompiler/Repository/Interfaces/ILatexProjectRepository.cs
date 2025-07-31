using Core.Interfaces;
using LatexCompiler.Entities;

namespace LatexCompiler.Repository.Interfaces;

public interface ILatexProjectRepository: IRepository<LatexProject>
{
    Task<IEnumerable<LatexProject>> GetUserProjectsAsync(long userId, CancellationToken token);
}