namespace CodeRunner.Entities;

public class CppLanguageConfig: LanguageConfig
{
    public override string FileExtension => ".cpp";
    
    public override string MainFileName => $"main{FileExtension}";
    
    private const string OutFileName = "main";
    
    public async override Task<ProcessExecutionStep[]> GetExecutionStepsAsync(string tempDir)
    {
        var codeFilePath = Path.Combine(tempDir, MainFileName);
        var outFilePath = Path.Combine(tempDir, OutFileName);
        
        return 
        [
            new  ProcessExecutionStep
            {
                FileName = "g++",
                Arguments = $"{codeFilePath} -o {outFilePath}"
            },
            new ProcessExecutionStep
            {
                FileName = $"./{outFilePath}"
            }
        ];
    }
}