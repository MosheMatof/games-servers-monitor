using GamesServersMonitor.Infrastructure.Repository;
using GamesServersMonitor.App.Services;
using GamesServersMonitor.Domain.Contracts.Services;
using GamesServersMonitor.Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using GamesServersMonitor.Infrastructure.Hubs;
using RabbitMQ.Client;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GamesServersMonitor.Domain.Entities;
using GamesServersMonitor.Infrastructure.Messaging.RabbitMQ;
using GamesServersMonitor.Infrastructure.Messaging.MediatR;
using MediatR;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMyDbContext(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<ConnectionFactory>(sp =>
{
    var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
    var factory = new ConnectionFactory()
    {
        HostName = rabbitMqConfig.GetValue<string>("Host"),
        Port = rabbitMqConfig.GetValue<int>("Port"),
        UserName = rabbitMqConfig.GetValue<string>("UserName"),
        Password = rabbitMqConfig.GetValue<string>("Password"),
        VirtualHost = rabbitMqConfig.GetValue<string>("VirtualHost"),
        Uri = new Uri("amqp://guest:guest@localhost:6770"),
        ClientProvidedName = "GamesServersMonitor"
    };
    return factory;
});
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration["Redis"];
//});

//builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration["Redis"]));

builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddSingleton<IGetGamesService, GetGamesService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();


//builder.Services.AddSingleton<EmulatorService>();
//builder.Services.AddSingleton<IEmulatorService>(sp => sp.GetRequiredService<EmulatorService>());
//builder.Services.AddSingleton<IRequestHandler<ServerUpdateRequest>>(sp => sp.GetRequiredService<EmulatorService>());

builder.Services.AddSingleton<IEmulatorService, EmulatorService>();

builder.Services.AddSingleton<IRequestHandler<ServerUpdateRequest>,EmulatorRequestHandler>();

builder.Services.AddSingleton<IRequestHandler<ServerUpdateResponse>, ServerHub>();
builder.Services.AddMediatR(cfg =>
{ cfg.RegisterServicesFromAssemblyContaining(typeof(EmulatorService));
  cfg.Lifetime = ServiceLifetime.Singleton;
});
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(ServerHub)));

builder.Services.AddHttpClient<IEmulatorService, EmulatorService>(HttpClient =>
{
    HttpClient.BaseAddress = new Uri("http://localhost:5000/");
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.MapHub<ServerHub>("/serverHub");

app.Run();


