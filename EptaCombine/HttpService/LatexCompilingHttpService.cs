using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LatexCompiler.Contracts;
using LatexCompiler.Entities;
using LatexCompiler.Service.Interfaces;

namespace EptaCombine.HttpService;

public class LatexCompilingHttpService : ILatexCompilingService
{
    private readonly HttpClient _httpClient;
    private const string SessionKey = "LatexProjectId";
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public LatexCompilingHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LatexProject> UploadAsync(Stream zipStream, ISession session, CancellationToken token)
    {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(zipStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
        content.Add(streamContent, "zipFile", "project.zip");

        var response = await _httpClient.PostAsync("api/LatexCompiler/Upload", content, token);
        await EnsureSuccessStatusCode(response, token);

        var project = await response.Content.ReadFromJsonAsync<LatexProject>(_jsonSerializerOptions, token);
        if (project == null)
        {
            throw new InvalidOperationException("Failed to deserialize project from API response.");
        }

        session.SetString(SessionKey, project.Uuid.ToString());

        return project;
    }

    public async Task<string> GetMainTexContentAsync(ISession session, CancellationToken token)
    {
        var response = await _httpClient.GetAsync("api/latexcompiler/GetMainTexContent", token);
        await EnsureSuccessStatusCode(response, token);
        
        return await response.Content.ReadAsStringAsync(token);
    }

    public async Task UpdateMainTexAsync(ISession session, string content, CancellationToken token)
    {
        var request = new LatexRequest { Content = content };
        var response = await _httpClient.PostAsJsonAsync("api/latexcompiler/UpdateMainTex", request, _jsonSerializerOptions, token);
        await EnsureSuccessStatusCode(response, token);
    }

    public async Task<Stream> CompileAsync(ISession session, CancellationToken token)
    {
        var response = await _httpClient.GetAsync("api/latexcompiler/Compile", token);
        await EnsureSuccessStatusCode(response, token);

        var resultStream = new MemoryStream();
        await response.Content.CopyToAsync(resultStream, token);
        resultStream.Position = 0;

        return resultStream;
    }

    public async Task CleanupAsync(ISession session, CancellationToken token)
    {
        var response = await _httpClient.DeleteAsync("api/latexcompiler/Cleanup", token);
        await EnsureSuccessStatusCode(response, token);
    }

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response, CancellationToken token)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(token);
            throw new HttpRequestException($"API call failed with status code {response.StatusCode}: {errorContent}",
                null, response.StatusCode);
        }
    }
}