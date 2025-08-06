using Common.Entities.Enums;

namespace FileConverter.Service.Interfaces;

public interface IPandocService
{
    Task<Stream> ConvertFileAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken cancellationToken);
}