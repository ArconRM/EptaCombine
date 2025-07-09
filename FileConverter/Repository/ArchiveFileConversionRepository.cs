using FileConverter.Repository.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;
using System.IO.Compression;

namespace FileConverter.Repository;

public class ArchiveFileConversionRepository : IArchiveFileConversionRepository
{
    public async Task<Stream> ConvertArchiveAsync(
        Stream inputStream,
        ArchiveType inFormat,
        ArchiveType outFormat,
        CancellationToken cancellationToken)
    {
        using var archive = ArchiveFactory.Open(inputStream);
        var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();

        var output = new MemoryStream();

        if (outFormat == ArchiveType.GZip)
        {
            using var tarStream = new MemoryStream();
            using (var tarWriter =
                   WriterFactory.Open(tarStream, ArchiveType.Tar,
                       new WriterOptions(CompressionType.None) { LeaveStreamOpen = true }))
            {
                foreach (var entry in entries)
                {
                    using var entryStream = entry.OpenEntryStream();
                    using var ms = new MemoryStream();
                    entryStream.CopyTo(ms);
                    ms.Position = 0;
                    tarWriter.Write(entry.Key, ms, entry.LastModifiedTime ?? DateTime.Now);
                }
            }

            tarStream.Position = 0;

            var compressed = new GZipStream(output, CompressionMode.Compress, leaveOpen: true);

            await tarStream.CopyToAsync(compressed, cancellationToken);
            await compressed.DisposeAsync();
        }
        else
        {
            using var outWriter = WriterFactory.Open(output, outFormat,
                new WriterOptions(outFormat == ArchiveType.Tar ? CompressionType.None : CompressionType.Deflate)
                    { LeaveStreamOpen = true });
            foreach (var entry in entries)
            {
                using var entryStream = entry.OpenEntryStream();
                using var ms = new MemoryStream();
                entryStream.CopyTo(ms);
                ms.Position = 0;
                outWriter.Write(entry.Key, ms, entry.LastModifiedTime ?? DateTime.Now);
            }
        }

        output.Position = 0;
        return output;
    }
}