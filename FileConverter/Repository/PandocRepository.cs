using System.Diagnostics;
using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Repository.Interfaces;

namespace FileConverter.Repository;

public class PandocRepository : IPandocRepository
{
    public async Task<Stream> ConvertFileAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token)
    {
        string inFormatArgument = inFormat switch
        {
            FileFormat.Md => "markdown",
            FileFormat.Tex => "latex",
            _ => FileFormatExtensions.GetStringExtension(inFormat)
        };
        string outFormatArgument = outFormat switch
        {
            FileFormat.Md => "markdown",
            FileFormat.Tex => "latex",
            _ => FileFormatExtensions.GetStringExtension(outFormat)
        };
        
        var psi = new ProcessStartInfo {
            FileName = "pandoc",
            Arguments = $"-f {inFormatArgument} -t {outFormatArgument}",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        var proc = Process.Start(psi);
        
        await inputStream.CopyToAsync(proc.StandardInput.BaseStream, token);
        proc.StandardInput.Close();
        
        var outputStream = new MemoryStream();
        await proc.StandardOutput.BaseStream.CopyToAsync(outputStream, token);
        await proc.WaitForExitAsync(token);
        
        if (proc.ExitCode != 0)
            throw new InvalidOperationException(await proc.StandardError.ReadToEndAsync(token));
        
        outputStream.Position = 0;
        return outputStream;
    }
}