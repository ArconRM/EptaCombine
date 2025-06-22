using SixLabors.ImageSharp.Formats;

namespace FileConverter.Repository.Interfaces;

public interface IImageFileConversionRepository
{
    Task<Stream> ConvertFileAsync(
        Stream inputStream,
        IImageFormat inFormat,
        IImageFormat outFormat,
        CancellationToken cancellationToken);
}