using System.Net.Http.Headers;
using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Service.Interfaces;

namespace EptaCombine.HttpService;

public class FileConversionHttpService: IFileConversionService
{
    private readonly HttpClient _httpClient;

    public FileConversionHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Stream> ConvertFileAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(inputStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "inputFile", $"input.{FileFormatExtensions.GetStringExtension(inFormat)}");
        
        var url = $"Convert?inputFormat={(int)inFormat}&outFormat={(int)outFormat}";
        var response = await _httpClient.PostAsync(url, content, token);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(token);
            throw new InvalidOperationException($"Conversion failed: {error}");
        }

        var resultStream = new MemoryStream();
        await response.Content.CopyToAsync(resultStream, token);
        resultStream.Position = 0;

        return resultStream;
    }
}