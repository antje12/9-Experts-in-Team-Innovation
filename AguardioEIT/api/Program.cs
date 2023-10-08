using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigureApplicationParts(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureApplicationParts(IServiceCollection services)
{
    Assembly assembly = 
        Assembly.LoadFrom("C:\\Users\\nicol\\Desktop\\git\\9-Experts-in-Team-Innovation\\AguardioEIT\\Plugin\\bin\\Debug\\net7.0\\Plugin.dll");
    var part = new AssemblyPart(assembly);
    services.AddControllers().PartManager.ApplicationParts.Add(part);
}