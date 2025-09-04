namespace CodeRunner.Entities;

public class CSharpLanguageConfig : LanguageConfig
{
    public override string FileExtension => ".cs";
    public override string MainFileName => "Program.cs";

    public override async Task<ProcessExecutionStep[]> GetExecutionStepsAsync(string tempDir, string codeFilePath)
    {
        string projectPath = Path.Combine(tempDir, "Runner.csproj");
        
        return
        [
            new ProcessExecutionStep
            {
                FileName = "dotnet",
                Arguments = $"new console -n Runner -o \"{tempDir}\"",
                BeforeWriteToFile = true
            },
            new ProcessExecutionStep
            {
                FileName = "dotnet",
                Arguments = $"build \"{projectPath}\" --verbosity quiet"
            },
            new ProcessExecutionStep
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" --no-build"
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

            if (trimmed.Contains("error") && (trimmed.Contains(".cs(") || trimmed.Contains("CS")))
            {
                errorLines.Add(trimmed);
            }
            else if (trimmed.Contains("Build FAILED") || trimmed.Contains("error(s)"))
            {
                errorLines.Add(trimmed);
            }
        }

        return string.Join("\n", errorLines);
    }

    public override string ExtractWarnings(string output)
    {
        var lines = output.Split('\n');
        var errorLines = new SortedSet<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.Contains("warning") && (trimmed.Contains(".cs(") || trimmed.Contains("CS")) ||
                trimmed.Contains("warning(s)"))
            {
                errorLines.Add(trimmed);
            }
        }

        return string.Join("\n", errorLines);
    }
}
