using Common.Entities.Enums;

namespace FileConverter.Repository.Interfaces;

public interface IPandocRepository
{
    Task<Stream> ConvertFileAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken cancellationToken);
}