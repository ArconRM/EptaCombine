using System.ComponentModel.DataAnnotations;
using Common.Entities;
using Common.Entities.Enums;
using EptaCombine.HttpService.Interfaces;
using FileConverter.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EptaCombine.Pages;

[DisableRequestSizeLimit]
[ValidateAntiForgeryToken]
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
    
    public async Task<IActionResult> OnPostAnalyzeFileAsync([FromForm] IFormFile uploadFile)
    {
        if (uploadFile.Length == 0)
        {
            _logger.LogError("File is empty");
            return BadRequest("No file provided");
        }
       
        FileFormat? inputFormat = FileFormatExtensions.GetFormatFromFileName(uploadFile.FileName);
        if (inputFormat == null)
        {
            _logger.LogError($"{uploadFile.FileName} extension is not supported");
            return BadRequest("Unsupported file format");
        }

        FileCategory? category = SupportedFormats.GetCategory(inputFormat.Value);
        if (category == null)
        {
            return BadRequest("Unknown file category");
        }

        List<string> outputFormats = SupportedFormats.GetFormats(category.Value)
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

    public async Task<IActionResult> OnPostConvertFileAsync(
        [FromForm] IFormFile uploadFile,
        [FromForm] string outputFormat, 
        CancellationToken token)
    {
        try
        {
            await using Stream inputStream = uploadFile.OpenReadStream();
            FileFormat inputFormat = FileFormatExtensions.GetFormatFromFileName(uploadFile.FileName)!.Value;
            FileFormat outputFormatDecoded = FileFormatExtensions.GetFormatFromFileName(outputFormat)!.Value;
            outputFormat = FileFormatExtensions.GetStringExtension(outputFormatDecoded);
            
            Stream outputStream = await _fileConversionService.ConvertFileAsync(inputStream, inputFormat, outputFormatDecoded, token);
            string convertedFileName = Path.GetFileNameWithoutExtension(uploadFile.FileName) + $".{outputFormat}";
            return File(outputStream, "application/octet-stream", convertedFileName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}