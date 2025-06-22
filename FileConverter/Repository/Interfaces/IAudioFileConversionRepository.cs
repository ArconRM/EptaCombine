using Common.Entities.Enums;

namespace FileConverter.Repository.Interfaces;

public interface IAudioFileConversionRepository
{
    Task<Stream> ConvertAudioAsync(
        Stream inputStream,
        FileFormat outFormat,
        CancellationToken token);
}