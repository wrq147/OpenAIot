using FastTunnel.Core.Config;
using FastTunnel.Core.Extensions;
using FastTunnel.Core.Handlers.Server;
using FastTunnelServer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<SqlRespository>();
builder.Services.Configure<GeneralOption>(builder.Configuration.GetSection("General"));
builder.Services.AddFastTunnelServer(builder.Configuration.GetSection("FastTunnel"));
builder.Services.AddTransient<ILoginHandler, RemoteLoginHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseFastTunnelServer();
app.MapFastTunnelServer();

app.Run();
