using Common.Options;
using FileConverter.Repository;
using FileConverter.Repository.Interfaces;
using FileConverter.Service;
using FileConverter.Service.Interfaces;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

var maxFileSize = builder.Configuration
    .GetSection("FileUpload")
    .Get<FileUploadOptions>()
    .MaxFileSize;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFileConversionService, FileConversionService>();

builder.Services.AddScoped<IImageFileConversionRepository, ImageFileConversionRepository>();
builder.Services.AddScoped<IImageFileConversionService, ImageFileConversionService>();

builder.Services.AddScoped<IVideoFileConversionRepository, VideoFileConversionRepository>();
builder.Services.AddScoped<IVideoFileConversionService, VideoFileConversionService>();

builder.Services.AddScoped<IAudioFileConversionRepository, AudioFileConversionRepository>();
builder.Services.AddScoped<IAudioFileConversionService, AudioFileConversionService>();

builder.Services.AddScoped<IArchiveFileConversionRepository, ArchiveFileConversionRepository>();
builder.Services.AddScoped<IArchiveFileConversionService, ArchiveFileConversionService>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxFileSize;
    options.ValueLengthLimit = int.MaxValue; 
    options.MemoryBufferThreshold = int.MaxValue;
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = maxFileSize;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = maxFileSize;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();