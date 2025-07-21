using EptaCombine.HttpService;
using FileConverter.Service.Interfaces;
using LatexCompiler.Service.Interfaces;
using Microsoft.AspNetCore.Http.Features;

const long maxFileSize = 1_048_576_000; // 1 GB

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<IFileConversionService, FileConversionHttpService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5193/");
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpClient<ILatexCompilingService, LatexCompilingHttpService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5062/");
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