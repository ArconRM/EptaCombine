using Core.BaseEntities;
using LatexCompiler.Entities;
using LatexCompiler.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LatexCompiler.Repository;

public class LatexProjectRepository: BaseRepository<LatexProject>, ILatexProjectRepository
{
    private readonly LatexCompilerDbContext _context;

    public LatexProjectRepository(LatexCompilerDbContext context): base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LatexProject>> GetUserProjectsAsync(long userId, CancellationToken token)
    {
        return await _context.LatexProjects
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync(token);
    }
}