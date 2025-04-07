using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Audit.Core;
using Audit.Http;
using Fuse8.BackendInternship.PublicApi.Middleware;
using Fuse8.BackendInternship.PublicApi.Models;
using Fuse8.BackendInternship.PublicApi.Models.Exceptions;
using Fuse8.BackendInternship.PublicApi.Models.ModelBinders;
using Fuse8.BackendInternship.PublicApi.Services;
using Fuse8.BackendInternship.PublicApi.SwaggerFilters;
using gRPC;
using Microsoft.Extensions.DependencyInjection;

namespace Fuse8.BackendInternship.PublicApi;

public class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{

        services.Configure<DefaultSettings>(_configuration.GetSection("DefaultSettings"));
		services.Configure<DefaultSettings>(_configuration.GetSection("SecretSettings"));
        services.Configure<SecretSettings>(_configuration.GetSection("SecretSettings"));

        services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
			options.ModelBinderProviders.Insert(0, new DateBinderProvider());
        })

			// Добавляем глобальные настройки для преобразования Json
			.AddJsonOptions(
				options =>
				{
					// Добавляем конвертер для енама
					// По умолчанию енам преобразуется в цифровое значение
					// Этим конвертером задаем перевод в строковое значение
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
					options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                });


		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
			options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
			options.OperationFilter<HttpCodesDocOpFilter>();
			options.DocumentFilter<ErrorResponseDocumentFilter>();
			options.OperationFilter<JsonMediaTypeOperationFilter>();
        });
		Audit.Core.Configuration.Setup().UseSerilog(
			config=> config.LogLevel(auditEvent =>
			{
				if(auditEvent is AuditEventHttpClient)
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
								Length:>MaxAuditContentLength
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
			if(httpAction is not null)
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

		services.AddTransient<GrpcCurrencyService>();

		services.AddGrpcClient<gRPCCurrency.gRPCCurrencyClient>(grpc =>
		{
			grpc.Address = new Uri(_configuration.GetValue<string>("GrpcURL"));
		}).AddAuditHandler(audit => audit
            .IncludeRequestBody()
            .IncludeRequestHeaders()
            .IncludeResponseBody()
            .IncludeResponseHeaders()
            .IncludeContentHeaders());

		services.AddTransient<IncomingRequestsLogger>();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency API v1");
					options.RoutePrefix = string.Empty; 
				});
		}

		app.UseMiddleware<IncomingRequestsLogger>();

		app.UseRouting()
			.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}