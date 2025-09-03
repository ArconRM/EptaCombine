using System.Diagnostics;
using System.Text;
using CodeRunner.Repository.Interfaces;
using Common.DTO;

namespace CodeRunner.Repository;

public class CSharpCodeRunnerRepository : ICSharpCodeRunnerRepository
{
    public async Task<CodeExecutionResult> RunCodeAsync(string code, CancellationToken token)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "DotnetRunner_" + Guid.NewGuid());
        Console.WriteLine(tempDir);
        Directory.CreateDirectory(tempDir);
        try
        {
            var result = new List<string>();

            string projectPath = Path.Combine(tempDir, "Runner.csproj");
            string programPath = Path.Combine(tempDir, "Program.cs");

            var createProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"new console -n Runner -o \"{tempDir}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                }
            };

            createProcess.Start();
            await createProcess.WaitForExitAsync(token);

            if (createProcess.ExitCode != 0)
            {
                string createError = await createProcess.StandardError.ReadToEndAsync(token);
                return new CodeExecutionResult()
                {
                    IsSuccess = false,
                    Output = $"Project creation failed: {createError}"
                };
            }

            await File.WriteAllTextAsync(programPath, code, token);

            var buildProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"build \"{projectPath}\" --verbosity quiet",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                }
            };

            buildProcess.Start();
            string buildOutput = await buildProcess.StandardOutput.ReadToEndAsync(token);
            string buildError = await buildProcess.StandardError.ReadToEndAsync(token);
            await buildProcess.WaitForExitAsync(token);

            if (buildProcess.ExitCode != 0)
            {
                return new CodeExecutionResult()
                {
                    IsSuccess = false,
                    Output = ExtractErrors(buildOutput)
                };
            }
            else
            {
                var warnings = ExtractWarnings(buildOutput);
                if (warnings.Any()) result.Add(warnings);
            }

            var runProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --project \"{projectPath}\" --no-build",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                }
            };

            runProcess.Start();
            string runOutput = await runProcess.StandardOutput.ReadToEndAsync(token);
            string runError = await runProcess.StandardError.ReadToEndAsync(token);
            await runProcess.WaitForExitAsync(token);

            if (runProcess.ExitCode != 0)
            {
                result.Add(runError);
                return new CodeExecutionResult()
                {
                    IsSuccess = false,
                    Output = string.Join("\n", result)
                };
            }
            
            result.Add(runOutput);
            return new CodeExecutionResult()
            {
                IsSuccess = true,
                Output = string.Join("\n", result)
            };
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    private string ExtractErrors(string output)
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

    private string ExtractWarnings(string output)
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