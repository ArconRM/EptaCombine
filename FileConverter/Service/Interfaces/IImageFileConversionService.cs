using Common.Entities;
using Common.Entities.Enums;

namespace FileConverter.Service.Interfaces;

public interface IImageFileConversionService
{
    Task<Stream> ConvertImageAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken cancellationToken);
}