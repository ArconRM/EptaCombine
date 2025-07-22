using Common.Entities.Enums;

namespace EptaCombine.HttpService.Interfaces;

public interface IFileConversionHttpService
{
    Task<Stream> ConvertFileAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token);
}