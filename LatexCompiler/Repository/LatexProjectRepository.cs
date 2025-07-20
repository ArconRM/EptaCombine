using Core.BaseEntities;
using LatexCompiler.Entities;
using LatexCompiler.Repository.Interfaces;

namespace LatexCompiler.Repository;

public class LatexProjectRepository: BaseRepository<LatexProject>, ILatexProjectRepository
{
    private readonly LatexCompilerDbContext _context;

    public LatexProjectRepository(LatexCompilerDbContext context): base(context)
    {
        _context = context;
    }
}