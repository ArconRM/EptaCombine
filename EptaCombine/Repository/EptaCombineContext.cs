using EptaCombine.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EptaCombine.Repository;

public class EptaCombineContext: IdentityDbContext<User, IdentityRole<long>, long>
{
    public EptaCombineContext(DbContextOptions<EptaCombineContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}