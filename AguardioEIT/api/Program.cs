using System.Reflection;
using api;
using api.Service;
using Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IApplicationService, ApplicationService>();
builder.Services.LoadPlugins(builder.Configuration);

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

// void ConfigureApplicationParts(IServiceCollection services)
// {
    // Assembly assembly = 
    //     Assembly.LoadFrom("C:\\Users\\nicol\\Desktop\\git\\9-Experts-in-Team-Innovation\\AguardioEIT\\Plugin\\bin\\Debug\\net7.0\\Plugin.dll");
    // var part = new AssemblyPart(assembly);
    // services.AddControllers().PartManager.ApplicationParts.Add(part);
    //
    // var atypes = assembly.GetTypes();
    // var pluginClass = atypes.SingleOrDefault(t => t.GetInterface(nameof(IPlugin)) != null);
    //
    // if (pluginClass != null)
    // {
    //     var initMethod = pluginClass.GetMethod(nameof(IPlugin.Initialize), 
    //         BindingFlags.Public | BindingFlags.Instance);
    //     var obj = Activator.CreateInstance(pluginClass);
    //     initMethod.Invoke(obj, new object[] { services });
    // }
    //
    // Assembly assembly2 = Assembly.LoadFrom
    //     ("C:\\Users\\nicol\\Desktop\\git\\9-Experts-in-Team-Innovation\\AguardioEIT\\Plugin2\\bin\\Debug\\net7.0\\Plugin2.dll");
    // var part2 = new AssemblyPart(assembly2);
    // services.AddControllers().PartManager.ApplicationParts.Add(part2);
    //
    // var atypes2 = assembly2.GetTypes();
    // var pluginClass2 = atypes2.SingleOrDefault(t => t.GetInterface(nameof(IPlugin)) != null);
    //
    // if (pluginClass2 != null)
    // {
    //     var initMethod = pluginClass2.GetMethod(nameof(IPlugin.Initialize), 
    //         BindingFlags.Public | BindingFlags.Instance);
    //     var obj = Activator.CreateInstance(pluginClass2);
    //     initMethod.Invoke(obj, new object[] { services });
    // }
// }