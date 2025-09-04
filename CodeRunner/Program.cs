using CodeRunner.Repository;
using CodeRunner.Repository.Interfaces;
using CodeRunner.Service;
using CodeRunner.Service.Interfaces;
using Common.Entities.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKeyedScoped<ICodeRunnerRepository, CSharpCodeRunnerRepository>(ProgramLanguage.CSharp);
builder.Services.AddScoped<ICodeRunnerService, CodeRunnerService>();

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