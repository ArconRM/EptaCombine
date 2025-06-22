using Common.Entities.Enums;

namespace Common.Entities;

public static class FileFormatExtensions
{
    
    private static readonly Dictionary<string, FileFormat> ExtensionToFormat = SupportedFormats.GetAllMappings()
        .SelectMany(kv => kv.Value)
        .ToDictionary(fmt => fmt.ToString().ToLower(), fmt => fmt);

    public static FileFormat? GetFormatFromFileName(string fileName)
    {
        var ext = fileName.Split(".").Last().ToLower();
        return ExtensionToFormat.TryGetValue(ext, out var format) ? format : null;
    }
    
    public static string GetStringExtension(FileFormat format)
    {
        if (format == FileFormat.Hls)
            return "mp4";

        return format.ToString().ToLower();
    }
}