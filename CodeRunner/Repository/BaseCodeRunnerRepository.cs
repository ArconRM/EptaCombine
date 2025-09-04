using System.Diagnostics;
using System.Text;
using CodeRunner.Entities;
using CodeRunner.Repository.Interfaces;
using Common.DTO;

namespace CodeRunner.Repository;

public abstract class BaseCodeRunnerRepository : ICodeRunnerRepository
{
    private readonly ILogger<BaseCodeRunnerRepository> _logger;

    protected BaseCodeRunnerRepository(ILogger<BaseCodeRunnerRepository> logger)
    {
        _logger = logger;
    }
    
    protected abstract LanguageConfig GetLanguageConfig();

    public async Task<CodeExecutionResult> RunCodeAsync(string code, CancellationToken token)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), $"{GetType().Name}_{Guid.NewGuid()}");
        Console.WriteLine(tempDir);
        Directory.CreateDirectory(tempDir);

        try
        {
            var config = GetLanguageConfig();
            var result = new List<string>();

            string codeFilePath = Path.Combine(tempDir, config.MainFileName);

            var executionSteps = await config.GetExecutionStepsAsync(tempDir, codeFilePath);
            bool didWriteToFile = false;

            foreach (var step in executionSteps)
            {
                if (!didWriteToFile && !step.BeforeWriteToFile)
                {
                    didWriteToFile = true;
                    await File.WriteAllTextAsync(codeFilePath, code, token);
                }
                var processResult = await ExecuteProcessAsync(step, token);

                if (!processResult.IsSuccess)
                {
                    _logger.LogError(processResult.Output);
                    return new CodeExecutionResult
                    {
                        IsSuccess = false,
                        Output = $"Failed with: {config.ExtractErrors(processResult.Output)}"
                    };
                }
                else
                {
                    var warning = config.ExtractWarnings(processResult.Output);
                    if (!string.IsNullOrEmpty(warning))
                        result.Add(warning);

                    if (step == executionSteps.Last())
                        result.Add(processResult.Output);
                }
            }

            return new CodeExecutionResult
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

    private async Task<ProcessExecutionResult> ExecuteProcessAsync(ProcessExecutionStep step, CancellationToken token)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = step.FileName,
                Arguments = step.Arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            }
        };

        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync(token);
        string error = await process.StandardError.ReadToEndAsync(token);
        await process.WaitForExitAsync(token);

        return new ProcessExecutionResult
        {
            IsSuccess = process.ExitCode == 0,
            Output = process.ExitCode == 0 ? output : error,
            ExitCode = process.ExitCode
        };
    }

    private class ProcessExecutionResult
    {
        public bool IsSuccess { get; set; }
        public string Output { get; set; } = string.Empty;
        public int ExitCode { get; set; }
    }
}