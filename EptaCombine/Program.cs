using Common.Options;
using EptaCombine.Entities;
using EptaCombine.Entities.Utils;
using EptaCombine.Extensions;
using EptaCombine.HttpService;
using EptaCombine.HttpService.Interfaces;
using EptaCombine.Repository;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SessionOptions = Common.Options.SessionOptions;

var builder = WebApplication.CreateBuilder(args);

var servicesConfig = builder.Configuration.GetSection("Services");
var fileConverterUrl = servicesConfig["FileConverter"];
var latexCompilerUrl = servicesConfig["LatexCompiler"];

var maxFileSize = builder.Configuration
    .GetSection("FileUpload")
    .Get<FileUploadOptions>()
    .MaxFileSize;

builder.Services.AddRazorPages();

builder.Services.AddDbContext<EptaCombineContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

builder.Services.AddIdentity<User, IdentityRole<long>>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;

        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<EptaCombineContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth";
    // options.LogoutPath = "/Account/Logout";
    // options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

builder.Services.TryAddScoped<SignInManager<User>>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireRole(UserRoles.User));

    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole(UserRoles.Admin));
});

builder.Services.AddHttpClient<IFileConversionHttpService, FileConversionHttpService>(client =>
{
    client.BaseAddress = new Uri(fileConverterUrl);
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpClient<ILatexCompilingHttpService, LatexCompilingHttpService>(client =>
{
    client.BaseAddress = new Uri(latexCompilerUrl);
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

var keyRingPath = builder.Configuration["ASPNETCORE_DATA_PROTECTION__KEY_RING_PATH"];
if (!string.IsNullOrEmpty(keyRingPath))
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keyRingPath))
        .SetApplicationName("EptaCombineApp");
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EptaCombineContext>();
    dbContext.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

await app.SeedRolesAsync();

app.Run();