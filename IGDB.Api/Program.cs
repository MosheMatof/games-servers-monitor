using IGDB;
using IGDB.Api.Services;
using IGDB.Domain.Contracts;
using IGDB.Infrastructure.Massaging.Events;
using IGDB.Infrastructure.Massaging.Services;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Secrets.json", optional: true);

builder.Services.AddSingleton(x => 
    new IGDBClient(
        builder.Configuration.GetSection("IGDB:ClientId").Value,
        builder.Configuration.GetSection("IGDB:ClientSecret").Value
        )
);


builder.Services.AddSingleton<IDownloadService, DownloadService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

builder.Services.AddHttpClient<IDownloadService, DownloadService>(HttpClient =>
{
    HttpClient.BaseAddress = new Uri("https://images.igdb.com/");
});

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
        ClientProvidedName = "IGDB"
    };
    return factory;
});

builder.Services.AddHostedService<GetNewGamesEventHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();


