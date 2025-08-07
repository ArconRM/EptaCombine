using Common.Entities.Enums;
using SharpCompress.Common;

namespace FileConverter.Repository.Interfaces;

public interface IArchiveFileConversionRepository
{
    Task<Stream> ConvertArchiveAsync(
        Stream inputStream,
        ArchiveType inFormat,
        ArchiveType outFormat,
        CancellationToken token);
}