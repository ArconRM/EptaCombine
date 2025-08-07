using System.IO.Compression;
using Common.Entities;
using Common.Entities.Enums;
using FileConverter.Repository.Interfaces;
using FileConverter.Service.Interfaces;

namespace FileConverter.Service;

public class FileConversionService : IFileConversionService
{
    private readonly IImageFileConversionService _imageFileConversionService;
    private readonly IVideoFileConversionService _videoFileConversionService;
    private readonly IAudioFileConversionService _audioFileConversionService;
    private readonly IArchiveFileConversionService _archiveFileConversionService;
    private readonly IPandocService _pandocService;

    public FileConversionService(
        IImageFileConversionService imageFileConversionService,
        IVideoFileConversionService videoFileConversionService,
        IAudioFileConversionService audioFileConversionService,
        IArchiveFileConversionService archiveFileConversionService,
        IPandocService pandocService)
    {
        _imageFileConversionService = imageFileConversionService;
        _videoFileConversionService = videoFileConversionService;
        _audioFileConversionService = audioFileConversionService;
        _archiveFileConversionService = archiveFileConversionService;
        _pandocService = pandocService;
    }

    public async Task<Stream> ConvertFileAsync(
        Stream inputStream,
        FileFormat inFormat,
        FileFormat outFormat,
        CancellationToken token)
    {
        FileCategory inCategory = SupportedFormats.GetCategory(inFormat);
        FileCategory outCategory = SupportedFormats.GetCategory(outFormat);

        if (inCategory != outCategory)
            throw new ArgumentException($"Categories {inCategory} and {outCategory} are different");

        switch (inCategory)
        {
            case FileCategory.Image:
                return await _imageFileConversionService.ConvertImageAsync(
                    inputStream,
                    inFormat,
                    outFormat,
                    token);

            case FileCategory.Archive:
                return await _archiveFileConversionService.ConvertArchiveAsync(
                    inputStream,
                    inFormat,
                    outFormat,
                    token);

            case FileCategory.Video:
                return await _videoFileConversionService.ConvertVideoAsync(
                    inputStream,
                    inFormat,
                    outFormat,
                    token);

            case FileCategory.Audio:
                return await _audioFileConversionService.ConvertAudioAsync(
                    inputStream,
                    outFormat,
                    token);
            
            case FileCategory.Docs:
            case FileCategory.Books:
                return await _pandocService.ConvertFileAsync(
                    inputStream,
                    inFormat,
                    outFormat,
                    token);
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    
    public async Task<Stream> ConvertFilesInArchiveAsync(
        Stream inputStream,
        FileFormat outFormat,
        CancellationToken token)
    {
        using var zipArchive = new ZipArchive(inputStream, ZipArchiveMode.Read);
        
        var requiredCategory = SupportedFormats.GetCategory(outFormat);
        var zipCategories = zipArchive.Entries
            .Select(x => SupportedFormats.GetCategory(
                FileFormatExtensions.GetFormatFromFileName(
                    Path.GetExtension(x.Name)) ?? FileFormat.Unknown)
            )
            .Distinct()
            .ToList();
        if (zipCategories.Count != 1 && zipCategories.First() != requiredCategory)
            throw new InvalidDataException("Zip contains more than one file format category");
        
        var outputStream = new MemoryStream();
        using (var outputZip = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var entry in zipArchive.Entries)
            {
                if (entry.Name.StartsWith("."))
                    continue;
            
                var fileFormat = FileFormatExtensions.GetFormatFromFileName(entry.Name).Value;
                await using var entryStream = entry.Open();
                var convertedEntry = await ConvertFileAsync(entryStream, fileFormat, outFormat, token);
            
                var newEntry = outputZip.CreateEntry(string.Format(
                    "{0}.{1}", 
                    Path.GetFileNameWithoutExtension(entry.Name), 
                    FileFormatExtensions.GetStringExtension(outFormat))
                );
                await using var newEntryStream = newEntry.Open();
                await convertedEntry.CopyToAsync(newEntryStream, token);
            }
        }
        
        outputStream.Position = 0;
        return outputStream;
    }
}