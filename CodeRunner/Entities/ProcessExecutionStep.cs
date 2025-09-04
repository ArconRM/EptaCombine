namespace CodeRunner.Entities;

public class ProcessExecutionStep
{
    public string FileName { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;

    public bool BeforeWriteToFile { get; set; } = false;
}