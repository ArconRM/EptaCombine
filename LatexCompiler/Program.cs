using Common.Options;
using LatexCompiler.Automapper;
using LatexCompiler.Options;
using LatexCompiler.Repository;
using LatexCompiler.Repository.Interfaces;
using LatexCompiler.Service;
using LatexCompiler.Service.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using SessionOptions = Common.Options.SessionOptions;

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

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ILatexProjectRepository, LatexProjectRepository>();

builder.Services.AddScoped<ILatexCompilingRepository, LatexCompilingRepository>();
builder.Services.AddScoped<ILatexCompilingService, LatexCompilingService>();

builder.Services.AddDbContext<LatexCompilerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<CompilerSettings>(
    builder.Configuration.GetSection("CompilerSettings"));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxFileSize;
    options.ValueLengthLimit = int.MaxValue; 
    options.MemoryBufferThreshold = int.MaxValue;
});

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

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = maxFileSize;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = maxFileSize;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LatexCompilerDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();
app.MapControllers();
app.Run();