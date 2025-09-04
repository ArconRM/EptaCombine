using Common.Entities.Enums;

namespace Common.DTO;

public class CodeExecutionRequest
{
    public string Code { get; set; }

    public ProgramLanguage ProgramLanguage { get; set; }
}