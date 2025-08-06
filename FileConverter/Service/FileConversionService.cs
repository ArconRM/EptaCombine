using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Repository.Interfaces;
using FileConverter.Service.Interfaces;

namespace FileConverter.Service;

public class FileConversionService : IFileConversionService
{
    private readonly IImageFileConversionService _imageFileConversionService;
    private readonly IVideoFileConversionService _videoFileConversionService;
    private readonly IAudioFileConversionService _audioFileConversionService;
    private readonly IArchiveFileConversionService _archiveFileConversionService;
    private readonly IPandocService _pandocService;

    public FileConversionService(
        IImageFileConversionService imageFileConversionService,
        IVideoFileConversionService videoFileConversionService,
        IAudioFileConversionService audioFileConversionService,
        IArchiveFileConversionService archiveFileConversionService,
        IPandocService pandocService)
    {
        _imageFileConversionService = imageFileConversionService;
        _videoFileConversionService = videoFileConversionService;
        _audioFileConversionService = audioFileConversionService;
        _archiveFileConversionService = archiveFileConversionService;
        _pandocService = pandocService;
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

            case FileCategory.Archive:
                return await _archiveFileConversionService.ConvertArchiveAsync(
                    inputStream,
                    inFormat,
                    outFormat,
                    token);

            case FileCategory.Video:
                return await _videoFileConversionService.ConvertVideoAsync(
                    inputStream,
                    inFormat,
                    outFormat,
                    token);

            case FileCategory.Audio:
                return await _audioFileConversionService.ConvertAudioAsync(
                    inputStream,
                    outFormat,
                    token);
            
            case FileCategory.Docs:
            case FileCategory.Books:
                return await _pandocService.ConvertFileAsync(
                    inputStream,
                    inFormat,
                    outFormat,
                    token);
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}