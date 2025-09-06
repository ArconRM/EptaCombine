namespace CodeRunner.Entities;

public class CppLanguageConfig: LanguageConfig
{
    public override string FileExtension => ".cpp";
    
    public override string MainFileName => "main.cpp";
    
    private const string OutFileName = "main";
    
    public async override Task<ProcessExecutionStep[]> GetExecutionStepsAsync(string tempDir, string codeFilePath)
    {
        return 
        [
            new  ProcessExecutionStep
            {
                FileName = "g++",
                Arguments = $"{MainFileName} -o {OutFileName}"
            },
            new ProcessExecutionStep
            {
                FileName = $"./{OutFileName}"
            }
        ];
    }
}