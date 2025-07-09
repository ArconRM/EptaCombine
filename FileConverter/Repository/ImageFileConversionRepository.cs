using FileConverter.Repository.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;

namespace FileConverter.Repository;

public class ImageFileConversionRepository: IImageFileConversionRepository
{
    public async Task<Stream> ConvertImageAsync(
        Stream inputStream,
        IImageFormat inFormat,
        IImageFormat outFormat,
        CancellationToken token)
    {
        var outputStream = new MemoryStream();
    
        using (var image = await Image.LoadAsync(inputStream, token))
        {
            IImageEncoder encoder = outFormat switch
            {
                PngFormat _ => new PngEncoder(),
                JpegFormat _ => new JpegEncoder { Quality = 100 },
                BmpFormat _ => new BmpEncoder(),
                TiffFormat _ => new TiffEncoder(),
                WebpFormat _ => new WebpEncoder(),
                GifFormat _ => new GifEncoder(),
                _ => throw new NotSupportedException($"Format {outFormat.Name} not supported")
            };
        
            await image.SaveAsync(outputStream, encoder, token);
        }
    
        outputStream.Position = 0;
        return outputStream;
    }
}