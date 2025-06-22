using Common.Entities.Enums;

namespace FileConverter.Service.Interfaces;

public interface IAudioFileConversionService
{
    Task<Stream> ConvertAudioAsync(
        Stream inputStream,
        FileFormat outFormat,
        CancellationToken token);
}