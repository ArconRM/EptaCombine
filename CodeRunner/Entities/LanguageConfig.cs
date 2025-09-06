namespace CodeRunner.Entities;

public abstract class LanguageConfig
{
    public abstract string FileExtension { get; }
    public abstract string MainFileName { get; }
    public abstract Task<ProcessExecutionStep[]> GetExecutionStepsAsync(string tempDir);
    public virtual string ExtractErrors(string output) => output;
    public virtual string ExtractWarnings(string output) => string.Empty;
}