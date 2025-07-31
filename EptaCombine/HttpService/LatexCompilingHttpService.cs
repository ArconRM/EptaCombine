using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Common.DTO;
using EptaCombine.HttpService.Interfaces;

namespace EptaCombine.HttpService;

public class LatexCompilingHttpService : ILatexCompilingHttpService
{
    private readonly HttpClient _httpClient;
    private const string SessionKey = "LatexProjectId";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public LatexCompilingHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<LatexProjectDTO>> GetUserProjectsAsync(long userId, CancellationToken token)
    {
        var response = await _httpClient.GetAsync($"api/LatexCompiler/GetUserProjects?userId={userId}", token);
        await EnsureSuccessStatusCode(response, token);

        var projects =
            await response.Content.ReadFromJsonAsync<IEnumerable<LatexProjectDTO>>(_jsonSerializerOptions, token);
        if (projects is null)
        {
            throw new InvalidOperationException("Failed to deserialize latex projects from API response.");
        }

        return projects;
    }

    public void SelectActiveProject(Guid projectUuid, ISession session)
    {
        session.SetString(SessionKey, projectUuid.ToString());
    }

    public async Task<LatexProjectDTO> UploadAsync(long userId, Stream zipStream, ISession session,
        CancellationToken token)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(userId.ToString()), "userId");

        var streamContent = new StreamContent(zipStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
        content.Add(streamContent, "zipFile", "project.zip");

        var response = await _httpClient.PostAsync("api/LatexCompiler/Upload", content, token);
        await EnsureSuccessStatusCode(response, token);

        var project = await response.Content.ReadFromJsonAsync<LatexProjectDTO>(_jsonSerializerOptions, token);
        if (project is null)
        {
            throw new InvalidOperationException("Failed to deserialize project from API response.");
        }
        
        SelectActiveProject(project.Uuid, session);

        return project;
    }

    public async Task<string> GetMainTexContentAsync(ISession session, CancellationToken token)
    {
        var uuid = GetProjectUuidFromSession(session);
        var response = await _httpClient.GetAsync($"api/LatexCompiler/GetMainTexContent?projectUuid={uuid}", token);
        await EnsureSuccessStatusCode(response, token);

        return await response.Content.ReadAsStringAsync(token);
    }

    public async Task<string> GetMainBibContentAsync(ISession session, CancellationToken token)
    {
        var uuid = GetProjectUuidFromSession(session);
        var response = await _httpClient.GetAsync($"api/LatexCompiler/GetMainBibContent?projectUuid={uuid}", token);
        await EnsureSuccessStatusCode(response, token);

        return await response.Content.ReadAsStringAsync(token);
    }

    public async Task UpdateProjectAsync(ISession session, string texContent, string bibContent,
        CancellationToken token)
    {
        var uuid = GetProjectUuidFromSession(session);

        var request = new LatexContentUpdateRequest()
        {
            ProjectUuid = uuid,
            TexContent = texContent,
            BibContent = bibContent
        };

        var response = await _httpClient.PostAsJsonAsync("api/LatexCompiler/UpdateProject", request,
            _jsonSerializerOptions, token);
        await EnsureSuccessStatusCode(response, token);
    }

    public async Task<Stream> CompileAsync(ISession session, CancellationToken token)
    {
        var uuid = GetProjectUuidFromSession(session);
        var response = await _httpClient.GetAsync($"api/LatexCompiler/Compile?projectUuid={uuid}", token);
        await EnsureSuccessStatusCode(response, token);

        var resultStream = new MemoryStream();
        await response.Content.CopyToAsync(resultStream, token);
        resultStream.Position = 0;
        return resultStream;
    }

    public async Task CleanupAsync(ISession session, CancellationToken token)
    {
        var uuid = GetProjectUuidFromSession(session);
        var requestUri = $"api/LatexCompiler/Cleanup?projectUuid={uuid}";
        var response = await _httpClient.DeleteAsync(requestUri, token);
        session.Remove(SessionKey);
        await EnsureSuccessStatusCode(response, token);
    }

    private static Guid GetProjectUuidFromSession(ISession session)
    {
        var uuidStr = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(uuidStr))
            throw new InvalidOperationException("Project UUID not found in session.");

        return Guid.Parse(uuidStr);
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