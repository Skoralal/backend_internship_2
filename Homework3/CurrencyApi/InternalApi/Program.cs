using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Audit.Core;
using Audit.Http;
using InternalApi.Models;
using InternalApi.Models.Exceptions;
using InternalApi.Services;
using InternalApi.SwaggerFilters;
using InterpolatedParsing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Filters;

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

var _configuration = builder.Configuration;

//Simple services
builder.Services.AddScoped<IncomingRequestsLogger>();
builder.Services.AddScoped<CacheService>();
builder.Services.AddScoped<CurrencyRequestHandlerService>();
builder.Services.Configure<AppOptions>(_configuration.GetSection("DefaultSettings"));
builder.Services.Configure<NetOptions>(_configuration.GetSection("SecretSettings"));
builder.Services.Configure<NetOptions>(_configuration.GetSection("NetOptions"));
builder.Services.AddOpenApi();
builder.Services.AddGrpc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
//Services with configuration
builder.Services.AddDbContext<CacheDBContext>(
    options =>
    {
        options.UseNpgsql(connectionString: _configuration.GetConnectionString("CurrencyApi"),
            npgsqlOptionsAction: optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure();
                optionsBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, CacheDBContext.SchemaName);
            }).UseSnakeCaseNamingConvention();
    });

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), includeControllerXmlComments: true);
    options.OperationFilter<HttpCodesDocOpFilter>();
    options.DocumentFilter<ErrorResponseDocumentFilter>();
    options.OperationFilter<JsonMediaTypeOperationFilter>();
});

builder.Services.AddHttpClient<HttpCallerService>().AddAuditHandler(audit => audit
    .IncludeRequestBody()
    .IncludeRequestHeaders()
    .IncludeResponseBody()
    .IncludeResponseHeaders()
    .IncludeContentHeaders());

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
})// Добавляем глобальные настройки для преобразования Json
    .AddJsonOptions(
        options =>
        {
            // Добавляем конвертер для енама
            // По умолчанию енам преобразуется в цифровое значение
            // Этим конвертером задаем перевод в строковое значение
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
//Other configuration
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



var app = builder.Build();

(int grcpPort, int httpPort) = GetPorts();
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
app.MapHealthChecks("/health");




app.UseWhen(
    predicate: context => context.Connection.LocalPort == grcpPort,
    configuration: grpcBuilder =>
    {
        grpcBuilder.UseRouting();
        grpcBuilder.UseEndpoints(endpoints => endpoints.MapGrpcService<gRPCServer>());
    }
    );

app.UseWhen(
    predicate: context => context.Connection.LocalPort == httpPort,
    configuration: httpBuilder =>
    {
        httpBuilder.UseRouting()
    .UseEndpoints(endpoints => endpoints.MapControllers());
    }
    );
app.MapControllers();



app.Run();

(int GrpcPort, int HttpPort) GetPorts()
{
    int grcpPort = ParsePortFromEndpoint("Grpc");
    int httpPort = ParsePortFromEndpoint("Http");
    int ParsePortFromEndpoint(string endpointKey)
    {
        var endpointUrl = _configuration.GetValue<string>($"Kestrel:Endpoints:{endpointKey}:Url") ?? throw new InvalidOperationException($"Could not find an endpoint with the key '{endpointKey}'");
        var schema = string.Empty;
        var url = string.Empty;
        var port = 0;
        InterpolatedParser.Parse(endpointUrl, $"{schema}://{url}:{port}");
        return port;
    }
    return (grcpPort, httpPort);
}

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