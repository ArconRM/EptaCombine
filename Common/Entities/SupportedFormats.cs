using Common.Entities.Enums;

namespace Common.Entities;

public static class SupportedFormats
{
    private static readonly Dictionary<FileCategory, FileFormat[]> Mapping = new()
    {
        { FileCategory.Image, [FileFormat.Png, FileFormat.Jpg, FileFormat.Jpeg, FileFormat.Bmp, FileFormat.Tiff, FileFormat.Webp] },
        { FileCategory.Document, [FileFormat.Pdf, FileFormat.Docx, FileFormat.Txt] },
        { FileCategory.Archive, [FileFormat.Zip, FileFormat.Tar, FileFormat.TarGZip] },
        { FileCategory.Video, [FileFormat.Mp4, FileFormat.Avi, FileFormat.MkV, FileFormat.Mov, FileFormat.Hls, FileFormat.Gif] },
        { FileCategory.Audio, [FileFormat.Mp3, FileFormat.Wav, FileFormat.Flac, FileFormat.Aac, FileFormat.Adts] },
    };

    public static FileCategory GetCategory(FileFormat format) =>
        Mapping.FirstOrDefault(kv => kv.Value.Contains(format)).Key;

    public static FileFormat[] GetFormats(FileCategory category) =>
        Mapping.TryGetValue(category, out var fmts) ? fmts : [];
    
    public static Dictionary<FileCategory, FileFormat[]> GetAllMappings() => Mapping;
}