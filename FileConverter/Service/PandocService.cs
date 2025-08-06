using Common.Entities.Enums;
using FileConverter.Repository.Interfaces;
using FileConverter.Service.Interfaces;

namespace FileConverter.Service;

public class PandocService: IPandocService
{
    private readonly IPandocRepository  _pandocRepository;

    public PandocService(IPandocRepository pandocRepository)
    {
        _pandocRepository = pandocRepository;
    }

    public async Task<Stream> ConvertFileAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken cancellationToken)
    {
        return await _pandocRepository.ConvertFileAsync(inputStream, inFormat, outFormat, cancellationToken);
    }
}