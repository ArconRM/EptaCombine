using System.ComponentModel.DataAnnotations.Schema;
using Core.Interfaces;

namespace LatexCompiler.Entities;

public class LatexProject: IEntity
{
    public Guid Uuid { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string ProjectDirectoryName { get; set; }
    
    public string MainTexPath => Path.Combine(ProjectDirectoryName, "main.tex");
}