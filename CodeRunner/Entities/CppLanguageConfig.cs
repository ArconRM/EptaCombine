namespace CodeRunner.Entities;

public class CppLanguageConfig : LanguageConfig
{
    public override string FileExtension => ".cpp";

    public override string MainFileName => $"main{FileExtension}";

    private const string OutFileName = "main";

    public async override Task<ProcessExecutionStep[]> GetExecutionStepsAsync(string tempDir)
    {
        var codeFilePath = Path.Combine(tempDir, MainFileName);
        var executablePath = Path.Combine(tempDir, OutFileName);

        return
        [
            new ProcessExecutionStep
            {
                FileName = "g++",
                Arguments = $"\"{codeFilePath}\" -o \"{executablePath}\"",
            },
            new ProcessExecutionStep
            {
                FileName = executablePath
            }
        ];
    }

    public override string ExtractErrors(string output)
    {
        var lines = output.Split('\n');
        var errorLines = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.Contains("error:") && (trimmed.Contains(".cpp:") || trimmed.Contains(".h:")))
            {
                errorLines.Add(trimmed);
            }
            else if (trimmed.Contains("undefined reference") ||
                     trimmed.Contains("unresolved external symbol") ||
                     trimmed.Contains("ld: ") ||
                     trimmed.Contains("collect2: error:"))
            {
                errorLines.Add(trimmed);
            }
            else if (trimmed.Contains("compilation terminated") ||
                     trimmed.Contains("failed") ||
                     trimmed.Contains("*** Error"))
            {
                errorLines.Add(trimmed);
            }
        }

        return string.Join("\n", errorLines);
    }

    public override string ExtractWarnings(string output)
    {
        var lines = output.Split('\n');
        var warningLines = new SortedSet<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.Contains("warning:") && (trimmed.Contains(".cpp:") || trimmed.Contains(".h:")))
            {
                warningLines.Add(trimmed);
            }
        }

        return string.Join("\n", warningLines);
    }
}