using Common.Entities.Enums;

namespace FileConverter.Service.Interfaces;

public interface IVideoFileConversionService
{
    Task<Stream> ConvertVideoAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token);
}