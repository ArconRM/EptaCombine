using System.IO.Compression;
using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Repository.Interfaces;
using FileConverter.Service.Interfaces;
using SharpCompress.Common;

namespace FileConverter.Service;

public class ArchiveFileConversionService : IArchiveFileConversionService
{
    private readonly IArchiveFileConversionRepository _archiveFileConversionRepository;

    public ArchiveFileConversionService(IArchiveFileConversionRepository archiveFileConversionRepository)
    {
        _archiveFileConversionRepository = archiveFileConversionRepository;
    }

    public async Task<Stream> ConvertArchiveAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token)
    {
        ArchiveType inType = MapArchiveFormat(inFormat);
        ArchiveType outType = MapArchiveFormat(outFormat);
        return await _archiveFileConversionRepository.ConvertArchiveAsync(
            inputStream,
            inType,
            outType,
            token);
    }

    private ArchiveType MapArchiveFormat(FileFormat fileFormat)
    {
        return fileFormat switch
        {
            FileFormat.Zip => ArchiveType.Zip,
            FileFormat.Tar => ArchiveType.Tar,
            FileFormat.TarGZip => ArchiveType.GZip,
            _ => throw new ArgumentOutOfRangeException($"Format {fileFormat} not supported")
        };
    }
}