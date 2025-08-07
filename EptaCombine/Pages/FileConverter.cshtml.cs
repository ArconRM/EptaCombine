using System.ComponentModel.DataAnnotations;
using Common.Entities;
using Common.Entities.Enums;
using EptaCombine.HttpService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EptaCombine.Pages;

[DisableRequestSizeLimit]
[ValidateAntiForgeryToken]
[Authorize]
public class FileConverterModel : PageModel
{
    private readonly ILogger<FileConverterModel> _logger;
    
    private readonly IFileConversionHttpService _fileConversionService;

    public FileConverterModel(ILogger<FileConverterModel> logger, IFileConversionHttpService fileConversionService)
    {
        _logger = logger;
        _fileConversionService = fileConversionService;
    }

    public void OnGet()
    {
    }
    
    public async Task<IActionResult> OnPostAnalyzeFileAsync(
        [FromForm] IFormFile uploadFile,
        [FromForm] bool convertFilesInArchive)
    {
        if (uploadFile.Length == 0)
        {
            _logger.LogError("File is empty");
            return BadRequest("No file provided");
        }
       
        FileFormat? inputFormat = FileFormatExtensions.GetFormatFromFileName(uploadFile.FileName);
        if (inputFormat == null)
            return BadRequest("Unsupported file format");

        if (inputFormat == FileFormat.Zip && convertFilesInArchive)
        {
            await using var stream = uploadFile.OpenReadStream();
            var category = _fileConversionService.AnalyzeZipArchiveCategory(stream);

            if (category == null)
                return BadRequest("Archive contents unsupported or mixed");

            var outputFormats = SupportedFormats.GetFormats(category.Value).Select(f => f.ToString()).ToList();

            return new JsonResult(new
            {
                category = category.ToString(),
                inputFormat = FileFormatExtensions.GetStringExtension(FileFormat.Zip),
                availableFormats = outputFormats
            });
        }
        else
        {
            var category = SupportedFormats.GetCategory(inputFormat.Value);
            var outputFormats = SupportedFormats.GetFormats(category)
                .Where(f => f != inputFormat.Value)
                .Select(f => f.ToString())
                .ToList();

            return new JsonResult(new
            {
                category = category.ToString(),
                inputFormat = inputFormat.ToString(),
                availableFormats = outputFormats
            });
        }
    }

    public async Task<IActionResult> OnPostConvertFileAsync(
        [FromForm] IFormFile uploadFile,
        [FromForm] string outputFormat,
        [FromForm] bool? convertFilesInArchive, 
        CancellationToken token)
    {
        try
        {
            await using var inputStream = uploadFile.OpenReadStream();
            FileFormat inputFormat = FileFormatExtensions.GetFormatFromFileName(uploadFile.FileName)!.Value;
            FileFormat outputFormatDecoded = FileFormatExtensions.GetFormatFromFileName(outputFormat)!.Value;

            Stream outputStream;
            string convertedFileName;

            if (inputFormat == FileFormat.Zip && convertFilesInArchive == true)
            {
                outputStream = await _fileConversionService.ConvertFilesInArchiveAsync(inputStream, outputFormatDecoded, token);
                convertedFileName = Path.GetFileName(uploadFile.FileName);
            }
            else
            {
                outputStream = await _fileConversionService.ConvertFileAsync(inputStream, inputFormat, outputFormatDecoded, token);
                convertedFileName = $"{Path.GetFileNameWithoutExtension(uploadFile.FileName)}.{outputFormat}";
            }
            
            return File(outputStream, "application/octet-stream", convertedFileName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}