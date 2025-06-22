using Common.Entities.Enums;

namespace FileConverter.Service.Interfaces;

public interface IFileConversionService
{
    Task<Stream> ConvertFileAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token);
}