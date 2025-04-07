using System.Text.Encodings.Web;
using Audit.Core;
using Audit.Http;
using InternalApi.Models;
using InternalApi.Services;
using InternalApi.SwaggerFilters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Filters;
using Serilog.Exceptions;
using InternalApi.Models.Exceptions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(
    (context, _, loggerConfig) =>
    {
        loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails(
            new DestructuringOptionsBuilder().WithFilter(
                new IgnorePropertyByNameExceptionFilter(
                    nameof(Exception.StackTrace),
                    nameof(Exception.Message),
                    nameof(Exception.TargetSite),
                    nameof(Exception.Source),
                    nameof(Exception.HResult),
                    "Type")
                )
            );
    }
);
// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var _configuration = builder.Configuration;
builder.Services.Configure<AppOptions>(_configuration.GetSection("DefaultSettings"));
builder.Services.Configure<NetOptions>(_configuration.GetSection("SecretSettings"));
builder.Services.Configure<NetOptions>(_configuration.GetSection("NetOptions"));
builder.Services.AddHttpClient<ExternalCallerService>().AddAuditHandler(audit => audit
        	.IncludeRequestBody()
            .IncludeRequestHeaders()
            .IncludeResponseBody()
            .IncludeResponseHeaders()
            .IncludeContentHeaders());
builder.Services.AddSingleton<CacheService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), includeControllerXmlComments:true);
    options.OperationFilter<HttpCodesDocOpFilter>();
    options.DocumentFilter<ErrorResponseDocumentFilter>();
    options.OperationFilter<JsonMediaTypeOperationFilter>();
});
Audit.Core.Configuration.Setup().UseSerilog(
    config => config.LogLevel(auditEvent =>
    {
        if (auditEvent is AuditEventHttpClient)
        {
            return Audit.Serilog.LogLevel.Debug;
        }
        return Audit.Serilog.LogLevel.Info;
    })
    .Message(auditEvent =>
    {
        auditEvent.Environment = null;
        const int MaxAuditContentLength = 10_000;
        if (auditEvent is AuditEventHttpClient httpClientEvent)
        {
            var responseContent = httpClientEvent.Action?.Response.Content;
            if (responseContent is
                {
                    Body: string
                    {
                        Length: > MaxAuditContentLength
                    }
                    bodyContent
                })
            {
                responseContent.Body = bodyContent[..MaxAuditContentLength] + "<...>";
            }
        }
        return auditEvent.ToJson();
    }));
Configuration.JsonSettings = new()
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};
Configuration.AddCustomAction(ActionType.OnEventSaving, HideSecrets);
void HideSecrets(AuditScope auditScope)
{
    var httpAction = auditScope.GetHttpAction();
    if (httpAction is not null)
    {
        HideHeadersSecret(httpAction.Request?.Headers);
        HideHeadersSecret(httpAction.Response?.Headers);
    }
    void HideHeadersSecret(IDictionary<string, string>? headers)
    {
        if (headers?.ContainsKey("apikey") is true)
        {
            headers["apikey"] = "*covert*";
        }
    }
}



builder.Services.AddTransient<IncomingRequestsLogger>();

var app = builder.Build();

var _netOptions = app.Services.GetRequiredService<IOptions<NetOptions>>().Value;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency API v1");
        options.RoutePrefix = string.Empty;
    });
}
app.UseMiddleware<IncomingRequestsLogger>();


app.UseWhen(
    predicate: context => context.Connection.LocalPort == _netOptions.gRPCPort,
    configuration: grpcBuilder =>
    {
        grpcBuilder.UseRouting();
        grpcBuilder.UseEndpoints(endpoints => endpoints.MapGrpcService<gRPCServer>());
    }
    );

app.UseWhen(
    predicate: context => context.Connection.LocalPort == _netOptions.HTTPPort,
    configuration: httpBuilder =>
    {
        httpBuilder.UseRouting()
    .UseEndpoints(endpoints => endpoints.MapControllers());
    }
    );
app.MapControllers();




app.Run();
