using System.Text.Json.Serialization;
using Audit.Core;
using Audit.Http;
using Fuse8.BackendInternship.PublicApi.Middleware;
using Fuse8.BackendInternship.PublicApi.Models;
using Fuse8.BackendInternship.PublicApi.Models.Exceptions;
using Fuse8.BackendInternship.PublicApi.Services;

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
        services.Configure<SecretSettings>(_configuration.GetSection("SecretSettings"));

        services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        })

			// Добавляем глобальные настройки для преобразования Json
			.AddJsonOptions(
				options =>
				{
					// Добавляем конвертер для енама
					// По умолчанию енам преобразуется в цифровое значение
					// Этим конвертером задаем перевод в строковое значение
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });


		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        Audit.Core.Configuration.Setup()
					.UseFileLogProvider(config =>
						config.Directory(@".\Logs")
							.FilenamePrefix("currency-api"));



		services.AddHttpClient<IExternalCallerService, ExternalCallerService>((caller, client) =>
        {
			client.BaseAddress = new Uri(_configuration["DefaultSettings:baseURL"]);
		})
		.AddAuditHandler(audit => audit
			.IncludeRequestBody()
			.IncludeRequestHeaders()
			.IncludeResponseBody()
			.IncludeResponseHeaders()
			.IncludeContentHeaders());
        Audit.Core.Configuration.CreationPolicy = EventCreationPolicy.InsertOnStartReplaceOnEnd;

		services.AddTransient<IncomingRequestsLogger>();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseMiddleware<IncomingRequestsLogger>();

		app.UseRouting()
			.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}