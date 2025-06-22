namespace FileConverter.Repository.Interfaces;

public interface IVideoFileConversionRepository
{
    Task<Stream> ConvertVideoAsync(
        Stream inputStream,
        string inFormat,
        string outFormat,
        CancellationToken token);
}