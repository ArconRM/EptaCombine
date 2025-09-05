namespace CodeRunner.Entities;

public class PythonLanguageConfig : LanguageConfig
{
    public override string FileExtension => ".py";
    public override string MainFileName => "main.py";

    public override async Task<ProcessExecutionStep[]> GetExecutionStepsAsync(string tempDir, string codeFilePath)
    {
        return
        [
            new ProcessExecutionStep
            {
                FileName = "python",
                Arguments = $"\"{codeFilePath}\""
            }
        ];
    }

    public override string ExtractErrors(string output)
    {
        return output;
    }

    public override string ExtractWarnings(string output)
    {
        var lines = output.Split('\n');
        var warningLines = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.Contains("Warning") || trimmed.Contains("DeprecationWarning") || 
                trimmed.Contains("FutureWarning") || trimmed.Contains("UserWarning"))
            {
                warningLines.Add(trimmed);
            }
        }

        return string.Join("\n", warningLines);
    }
}
