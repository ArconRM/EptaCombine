using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Repository.Interfaces;
using FileConverter.Service.Interfaces;

namespace FileConverter.Service;

public class AudioFileConversionService: IAudioFileConversionService
{
    private readonly IAudioFileConversionRepository _audioFileConversionRepository;

    public AudioFileConversionService(IAudioFileConversionRepository audioFileConversionRepository)
    {
        _audioFileConversionRepository = audioFileConversionRepository;
    }

    public async Task<Stream> ConvertAudioAsync(
        Stream inputStream,
        FileFormat outFormat,
        CancellationToken token)
    {
        return await _audioFileConversionRepository.ConvertAudioAsync(
            inputStream,
            outFormat,
            token); 
    }
}