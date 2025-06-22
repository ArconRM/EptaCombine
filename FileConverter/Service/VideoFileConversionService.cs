using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Repository.Interfaces;
using FileConverter.Service.Interfaces;

namespace FileConverter.Service;

public class VideoFileConversionService: IVideoFileConversionService
{
    private readonly IVideoFileConversionRepository _videoFileConversionRepository;

    public VideoFileConversionService(IVideoFileConversionRepository videoFileConversionRepository)
    {
        _videoFileConversionRepository = videoFileConversionRepository;
    }

    public async Task<Stream> ConvertVideoAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token)
    {
        string inFormatString = FileFormatExtensions.GetStringExtension(inFormat);
        string outFormatString = FileFormatExtensions.GetStringExtension(outFormat);
        return await _videoFileConversionRepository.ConvertVideoAsync(
            inputStream,
            inFormatString,
            outFormatString,
            token);
    }
}