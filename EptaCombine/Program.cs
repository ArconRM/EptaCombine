using EptaCombine.HttpService;
using FileConverter.Service.Interfaces;
using Microsoft.AspNetCore.Http.Features;

const long maxFileSize = 1_048_576_000; // 1 GB

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IFileConversionService, FileConversionHttpService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5193/");
    client.Timeout = TimeSpan.FromMinutes(10);
});

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();