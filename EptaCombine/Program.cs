using Common.Options;
using EptaCombine.HttpService;
using EptaCombine.HttpService.Interfaces;
using FileConverter.Service.Interfaces;
using LatexCompiler.Service.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using SessionOptions = Common.Options.SessionOptions;

var builder = WebApplication.CreateBuilder(args);

var maxFileSize = builder.Configuration
    .GetSection("FileUpload")
    .Get<FileUploadOptions>()
    .MaxFileSize;

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = builder.Configuration
        .GetSection("Session")
        .Get<SessionOptions>()
        .IdleTimeout;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<IFileConversionHttpService, FileConversionHttpService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7166/");
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpClient<ILatexCompilingHttpService, LatexCompilingHttpService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7294/");
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
app.UseSession();
app.MapRazorPages();

app.Run();