using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileConverter.Controllers;

public class ImageConverterController: Controller
{
    private readonly ILogger<ImageConverterController> _logger;
    private readonly IImageFileConversionService _imageFileConversionService;

    public ImageConverterController(
        ILogger<ImageConverterController> logger,
        IImageFileConversionService imageFileConversionService)
    {
        _logger = logger;
        _imageFileConversionService = imageFileConversionService;
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
            Stream outStream = await _imageFileConversionService.ConvertImageAsync(inputStream, inputFormat, outFormat, token);
            return Ok(outStream);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}