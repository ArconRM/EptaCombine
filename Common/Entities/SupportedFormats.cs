using Common.Entities.Enums;

namespace Common.Entities;

public static class SupportedFormats
{
    private static readonly Dictionary<FileCategory, FileFormat[]> Mapping = new()
    {
        { FileCategory.Image, [FileFormat.Png, FileFormat.Jpg, FileFormat.Jpeg, FileFormat.Bmp, FileFormat.Tiff, FileFormat.Webp] },
        { FileCategory.Archive, [FileFormat.Zip, FileFormat.Rar, FileFormat.Tar, FileFormat.TarGZip] },
        { FileCategory.Video, [FileFormat.Mp4, FileFormat.Avi, FileFormat.MkV, FileFormat.Mov, FileFormat.Gif, FileFormat.Webm] },
        { FileCategory.Audio, [FileFormat.Mp3, FileFormat.Wav, FileFormat.Flac, FileFormat.Aac, FileFormat.Adts] },
        { FileCategory.Docs, [FileFormat.Md, FileFormat.Html, FileFormat.Docx, FileFormat.Odt, FileFormat.Tex] },
        // { FileCategory.Books, [FileFormat.Epub, FileFormat.Fb2, FileFormat.Pdf] }
    };

    public static FileCategory GetCategory(FileFormat format) =>
        Mapping.FirstOrDefault(kv => kv.Value.Contains(format)).Key;

    public static FileFormat[] GetFormats(FileCategory category) =>
        Mapping.TryGetValue(category, out var fmts) ? fmts : [];
    
    public static Dictionary<FileCategory, FileFormat[]> GetAllMappings() => Mapping;
}