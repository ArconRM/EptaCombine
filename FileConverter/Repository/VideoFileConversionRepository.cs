using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using FileConverter.Repository.Interfaces;

namespace FileConverter.Repository;

public class VideoFileConversionRepository: IVideoFileConversionRepository
{
    public async Task<Stream> ConvertVideoAsync(
        Stream inputStream,
        string inFormat,
        string outFormat,
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
            
            var args = FFMpegArguments
                .FromFileInput(tempInputFile)
                .OutputToFile(tempOutputFile, true, opts => opts
                    .WithVideoCodec(VideoCodec.LibX264)
                    .WithAudioCodec(AudioCodec.Aac)
                    .ForceFormat(outFormat));
            
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
}