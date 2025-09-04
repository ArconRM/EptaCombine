using System.Text.Json;
using Common.DTO;
using Common.Entities.Enums;
using EptaCombine.HttpService.Interfaces;

namespace EptaCombine.HttpService;

public class CodeRunnerHttpsService: ICodeRunnerHttpsService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public CodeRunnerHttpsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CodeExecutionResult> RunCodeAsync(string code, ProgramLanguage language, CancellationToken token)
    {
        var request = new CodeExecutionRequest()
        {
            Code = code,
            ProgramLanguage = language
        };
        
        var response = await _httpClient.PostAsJsonAsync("api/CodeRunner/RunCode", request,
            _jsonSerializerOptions, token);
        await EnsureSuccessStatusCode(response, token);

        var result = await response.Content.ReadFromJsonAsync<CodeExecutionResult>(_jsonSerializerOptions, token);
        if (result is null)
        {
            throw new InvalidOperationException("Failed to deserialize project from API response.");
        }
        
        return result;
    }
    
    private async Task EnsureSuccessStatusCode(HttpResponseMessage response, CancellationToken token)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(token);
            throw new HttpRequestException(
                $"API call failed with status code {response.StatusCode}: {errorContent}",
                null,
                response.StatusCode
            );
        }
    }
}