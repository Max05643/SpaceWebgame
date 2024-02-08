using GameDesign.Utils;
using GameServersManager.Models;
using GameServersManager.Utils;
using Genbox.VelcroPhysics.Shared;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.DataProtection;
using WebInterface.Hubs;
using WebInterface.Utils;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var graphicLibrary = GraphicLibrary.LoadFromFile("graphicLib.json");
builder.Services.AddSingleton<IGraphicLibraryProvider, GraphicLibrary>((args) => graphicLibrary);

builder.Services.AddLogging(logBuilder =>
{

    if (bool.Parse(builder.Configuration["DiscordLogger:Enabled"] ?? "false"))
    {
        logBuilder.AddProvider(new DiscordLoggerProvider(builder.Configuration));
    }


    logBuilder.AddConsole();
    logBuilder.AddDebug();
});



builder.Services.AddSingleton<CaptchaValidator>();

builder.Services.AddStackExchangeRedisCache(opts =>
{
    opts.InstanceName = "Spacewar";
    opts.Configuration = builder.Configuration.GetConnectionString("redis_persistent");
}
);

builder.Services.AddDataProtection()
.PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("redis_persistent")!))
.SetApplicationName("Spacewar");


builder.Services.AddSession(opt =>
{
    opt.Cookie.SameSite = SameSiteMode.Lax;
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
}
);

builder.Services.AddSignalR().AddMessagePackProtocol(opts =>
{
    var resolver = CompositeResolver.Create(
    Vector2MPResolver.Instance,
    StandardResolver.Instance
    );
    opts.SerializerOptions = MessagePackSerializerOptions.Standard.WithResolver(resolver);
}
);



builder.Services.AddSingleton<IPlayersConnectionsStorage, RedisPlayersConnectionsStorage>();
builder.Services.AddSingleton<IChatStorage, RedisChatStorage>();




builder.Services.AddSingleton<FrontBackCommunication>();
builder.Services.AddSingleton<IMultiServerClientsCommunication>((services) => services.GetRequiredService<FrontBackCommunication>());


builder.Services.AddSingleton<IGameServersManager, GameServersManager.Utils.GameServersManager>();


builder.Services.AddSingleton<LoadBalancer>();

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new Vector2Converter());
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.Urls.Add("http://*:5000");
}



app.UseForwardedHeaders();

app.UseSession();


app.UsePlayerSessionMiddleware();

app.MapHub<GameHub>("/gamehub");


app.UseStaticFiles();

app.UseRouting();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

var logger = app.Services.GetService<ILogger<Program>>();

logger?.LogInformation("Using {environmentName} pipeline", app.Environment.EnvironmentName);

app.Run();
