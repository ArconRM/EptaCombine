using FileConverter.Repository;
using FileConverter.Repository.Interfaces;
using FileConverter.Service;
using FileConverter.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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