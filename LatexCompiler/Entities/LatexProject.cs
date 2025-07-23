using System.ComponentModel.DataAnnotations.Schema;
using Core.Interfaces;

namespace LatexCompiler.Entities;

public class LatexProject: IEntity
{
    public Guid Uuid { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string ProjectDirectory { get; set; }

    public string MainTexName { get; set; }

    public string MainBibName { get; set; }
    
    public string MainTexPath => Path.Combine(ProjectDirectory, MainTexName);

    public string MainBibPath => Path.Combine(ProjectDirectory, MainBibName);
}