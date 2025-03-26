using Fuse8.BackendInternship.PublicApi;
using Microsoft.AspNetCore;

var webHost = WebHost
	.CreateDefaultBuilder(args)
	.UseStartup<Startup>()
	.Build();

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true)
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var app = builder.Build();
await webHost.RunAsync();