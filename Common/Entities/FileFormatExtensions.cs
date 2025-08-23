using Common.Entities.Enums;

namespace Common.Entities;

public static class FileFormatExtensions
{
    
    private static readonly Dictionary<string, FileFormat> ExtensionToFormat = SupportedFormats.GetAllMappings()
        .SelectMany(kv => kv.Value)
        .ToDictionary(fmt => GetStringExtension(fmt), fmt => fmt);

    public static FileFormat? GetFormatFromFileName(string fileName)
    {
        var ext = fileName.Split(".").Last().ToLower();
        return ExtensionToFormat.TryGetValue(ext, out var format) ? format : null;
    }
    
    public static string GetStringExtension(FileFormat format)
    {
        if (format == FileFormat.Aac)
            return "m4a";

        if (format == FileFormat.TarGZip)
            return "tar.gz";

        return format.ToString().ToLower();
    }
}