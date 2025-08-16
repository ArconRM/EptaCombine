namespace Common.Entities.Enums;

public enum FileFormat
{
    Unknown,
    
    // Image
    Png,
    Jpg,
    Jpeg,
    Bmp,
    Tiff,
    Webp,

    // Archive
    Zip,
    Rar,
    Tar,
    TarGZip,
    
    // Video
    Mp4,
    Avi,
    MkV,
    Mov,
    Hls,
    Gif,
    Webm,

    // Audio
    Mp3,
    Wav,
    Flac,
    Aac,
    Adts,
    
    // Docs
    Md,
    Html,
    Docx,
    Odt,
    Tex,
    
    // Books
    Epub,
    Fb2,
    Pdf
}