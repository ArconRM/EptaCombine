namespace CodeRunner.Entities;

public class JSLanguageConfig : LanguageConfig
{
    public override string FileExtension => ".js";
    public override string MainFileName => $"script{FileExtension}";

    public override async Task<ProcessExecutionStep[]> GetExecutionStepsAsync(string tempDir)
    {
        var codeFilePath = Path.Combine(tempDir, MainFileName);
        
        return
        [
            new ProcessExecutionStep 
            {
                FileName = "node",
                Arguments = $"{codeFilePath}"
            }
        ];
    }
}