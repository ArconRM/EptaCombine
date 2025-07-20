using System.Reflection;
using Common.Entities;
using LatexCompiler.Entities;
using Microsoft.EntityFrameworkCore;

namespace LatexCompiler.Repository;

public class LatexCompilerDbContext: DbContext
{
    public DbSet<LatexProject> LatexProjects { get; set; }

    public LatexCompilerDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}