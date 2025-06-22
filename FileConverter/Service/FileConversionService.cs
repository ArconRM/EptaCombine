using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Service.Interfaces;

namespace FileConverter.Service;

public class FileConversionService : IFileConversionService
{
    private readonly IImageFileConversionService _imageFileConversionService;

    public FileConversionService(IImageFileConversionService imageFileConversionService)
    {
        _imageFileConversionService = imageFileConversionService;
    }

    public async Task<Stream> ConvertFileAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token)
    {
        FileCategory inCategory = SupportedFormats.GetCategory(inFormat);
        FileCategory outCategory = SupportedFormats.GetCategory(outFormat);

        if (inCategory != outCategory)
            throw new ArgumentException($"Categories {inCategory} and {outCategory} are different");
        
        switch (inCategory)
        {
            case FileCategory.Image: 
                return await _imageFileConversionService.ConvertImageAsync(
                    inputStream,
                    inFormat,
                    outFormat,
                    token);
            
            case FileCategory.Document:
                break;
            case FileCategory.Archive:
                break;
            case FileCategory.Video:
                break;
            case FileCategory.Audio:
                break;
        }
        
        return null;
    }
}