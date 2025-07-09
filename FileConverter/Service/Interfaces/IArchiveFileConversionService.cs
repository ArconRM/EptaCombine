using Common.Entities.Enums;

namespace FileConverter.Service.Interfaces;

public interface IArchiveFileConversionService
{
    Task<Stream> ConvertArchiveAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken cancellationToken);
}