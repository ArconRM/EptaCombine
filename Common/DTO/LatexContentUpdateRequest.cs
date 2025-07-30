namespace Common.DTO;

public class LatexContentUpdateRequest
{
    public Guid ProjectUuid { get; set; }
    
    public string TexContent { get; set; }

    public string BibContent { get; set; }
}
