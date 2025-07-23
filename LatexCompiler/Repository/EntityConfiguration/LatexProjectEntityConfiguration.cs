using LatexCompiler.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LatexCompiler.Repository.EntityConfiguration;

public class LatexProjectEntityConfiguration: IEntityTypeConfiguration<LatexProject>
{
    public void Configure(EntityTypeBuilder<LatexProject> builder)
    {
        builder.HasKey(lp => lp.Uuid);
        
        builder.Property(lp => lp.Uuid)
            .ValueGeneratedNever();
    }
}