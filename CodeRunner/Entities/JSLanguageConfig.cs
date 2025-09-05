namespace CodeRunner.Entities;

public class JSLanguageConfig : LanguageConfig
{
    public override string FileExtension => ".js";
    public override string MainFileName => "script.js";

    public override async Task<ProcessExecutionStep[]> GetExecutionStepsAsync(string tempDir, string codeFilePath)
    {
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