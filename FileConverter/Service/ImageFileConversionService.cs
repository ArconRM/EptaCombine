using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Repository.Interfaces;
using FileConverter.Service.Interfaces;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;

namespace FileConverter.Service;

public class ImageFileConversionService : IImageFileConversionService
{
    private readonly IImageFileConversionRepository _imageFileConversionRepository;

    public ImageFileConversionService(IImageFileConversionRepository imageFileConversionRepository)
    {
        _imageFileConversionRepository = imageFileConversionRepository;
    }

    public async Task<Stream> ConvertImageAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token)
    {
        IImageFormat inImageFormat = MapFileFormat(inFormat);
        IImageFormat outImageFormat = MapFileFormat(outFormat);
        return await _imageFileConversionRepository.ConvertImageAsync(inputStream, inImageFormat, outImageFormat, token);
    }

    private IImageFormat MapFileFormat(FileFormat fileFormat)
    {
        return fileFormat switch
        {
            FileFormat.Png => PngFormat.Instance,
            FileFormat.Jpeg or FileFormat.Jpg => JpegFormat.Instance,
            FileFormat.Bmp => BmpFormat.Instance,
            FileFormat.Tiff => TiffFormat.Instance,
            FileFormat.Webp => WebpFormat.Instance,
            FileFormat.Gif => GifFormat.Instance,
            _ => throw new ArgumentOutOfRangeException($"Format {fileFormat} not supported")
        };
    }
}