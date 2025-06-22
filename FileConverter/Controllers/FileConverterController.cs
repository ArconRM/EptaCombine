using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileConverter.Controllers;

public class FileConverterController: Controller
{
    private readonly ILogger<FileConverterController> _logger;
    private readonly IFileConversionService _fileConversionService;

    public FileConverterController(
        ILogger<FileConverterController> logger,
        IFileConversionService fileConversionService)
    {
        _logger = logger;
        _fileConversionService = fileConversionService;
    }

    [HttpPost(nameof(Convert))]
    public async Task<IActionResult> Convert(IFormFile inputFile, FileFormat inputFormat, FileFormat outFormat, CancellationToken token)
    {
        try
        {
            if (inputFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            await using var inputStream = inputFile.OpenReadStream();
            Stream outStream = await _fileConversionService.ConvertFileAsync(inputStream, inputFormat, outFormat, token);
            return Ok(outStream);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}