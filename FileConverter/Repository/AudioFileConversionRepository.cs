using Common.Entities;
using Common.Entities.Enums;
using FFMpegCore;
using FFMpegCore.Enums;
using FileConverter.Repository.Interfaces;

namespace FileConverter.Repository;

public class AudioFileConversionRepository : IAudioFileConversionRepository
{
    public async Task<Stream> ConvertAudioAsync(
        Stream inputStream,
        FileFormat outFormat,
        CancellationToken token)
    {
        var tempInputFile = Path.GetTempFileName();
        var tempOutputFile = Path.GetTempFileName();

        try
        {
            await using (var fileStream = File.Create(tempInputFile))
            {
                await inputStream.CopyToAsync(fileStream, token);
            }

            var outCodec = GetCodecForFormat(outFormat);
            var outFormatString = FileFormatExtensions.GetStringExtension(outFormat);

            var args = FFMpegArguments
                .FromFileInput(tempInputFile)
                .OutputToFile(tempOutputFile, true, opts => opts
                    .WithAudioCodec(outCodec)
                    .ForceFormat(outFormatString));

            await args.ProcessAsynchronously(throwOnError: true);

            var output = new MemoryStream();
            await using (var fileStream = File.OpenRead(tempOutputFile))
            {
                await fileStream.CopyToAsync(output, token);
            }

            output.Position = 0;
            return output;
        }
        finally
        {
            // Clean up temporary files
            if (File.Exists(tempInputFile))
                File.Delete(tempInputFile);
            if (File.Exists(tempOutputFile))
                File.Delete(tempOutputFile);
        }
    }

    private Codec GetCodecForFormat(FileFormat format)
    {
        switch (format)
        {
            case FileFormat.Mp3:
                return AudioCodec.LibMp3Lame;

            case FileFormat.Wav:
                return FFMpeg.GetCodec("pcm_s16le");

            case FileFormat.Flac:
                return FFMpeg.GetCodec("flac");

            case FileFormat.Aac:
            case FileFormat.Adts:
                return AudioCodec.Aac;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, $"Unknown audio codec: {format}");
        }
    }
}