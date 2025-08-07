using System.IO.Compression;
using System.Net.Http.Headers;
using Common.Entities;
using Common.Entities.Enums;
using EptaCombine.HttpService.Interfaces;

namespace EptaCombine.HttpService;

public class FileConversionHttpService: IFileConversionHttpService
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
        
        var url = $"api/fileconverter/Convert?inputFormat={(int)inFormat}&outFormat={(int)outFormat}";
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

    public async Task<Stream> ConvertFilesInArchiveAsync(
        Stream inputStream,
        FileFormat outFormat,
        CancellationToken token)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(inputStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "inputZip", $"input.zip");
        
        var url = $"api/fileconverter/ConvertFilesInArchive?&outFormat={(int)outFormat}";
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

    public FileCategory? AnalyzeZipArchiveCategory(Stream inputStream)
    {
        using var zipArchive = new ZipArchive(inputStream, ZipArchiveMode.Read);
        var zipCategories = zipArchive.Entries
            .Select(x => SupportedFormats.GetCategory(
                FileFormatExtensions.GetFormatFromFileName(
                    Path.GetExtension(x.Name)) ?? FileFormat.Unknown)
            )
            .Distinct()
            .ToList();
        if (zipCategories.Count != 1)
            return null;
        
        return zipCategories.First();
    }
}