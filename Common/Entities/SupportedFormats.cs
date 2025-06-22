using Common.Entities.Enums;

namespace Common.Entities;

public static class SupportedFormats
{
    private static readonly Dictionary<FileCategory, FileFormat[]> Mapping = new()
    {
        { FileCategory.Image, [FileFormat.Png, FileFormat.Jpg, FileFormat.Jpeg, FileFormat.Bmp, FileFormat.Tiff, FileFormat.Webp] },
        { FileCategory.Document, [FileFormat.Pdf, FileFormat.Docx, FileFormat.Txt] },
        { FileCategory.Archive, [FileFormat.Zip, FileFormat.TarGz] },
        { FileCategory.Video, [FileFormat.Mp4, FileFormat.Avi, FileFormat.MkV, FileFormat.Mov, FileFormat.Hls, FileFormat.Gif] },
        { FileCategory.Audio, [FileFormat.Mp3, FileFormat.Wav, FileFormat.Flac] },
    };
    
    private static readonly Dictionary<string, FileFormat> ExtensionToFormat = Mapping
        .SelectMany(kv => kv.Value)
        .ToDictionary(fmt => fmt.ToString().ToLower(), fmt => fmt);

    public static FileFormat? GetFormatFromFileName(string fileName)
    {
        var ext = fileName.Split(".").Last().ToLower();
        return ExtensionToFormat.TryGetValue(ext, out var format) ? format : null;
    }

    public static FileCategory GetCategory(FileFormat format) =>
        Mapping.FirstOrDefault(kv => kv.Value.Contains(format)).Key;

    public static FileFormat[] GetFormats(FileCategory category) =>
        Mapping.TryGetValue(category, out var fmts) ? fmts : [];
    
    public static Dictionary<FileCategory, FileFormat[]> GetAllMappings() => Mapping;
}