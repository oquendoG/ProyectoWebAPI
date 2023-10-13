using System.Net;
using System.Text.Json;

namespace API.Helpers.Errors;

public class ExceptionMiddleware
{
	private RequestDelegate Next { get; }
	private ILogger<ExceptionMiddleware> Logger { get; }
	private IHostEnvironment Env { get; }

	public ExceptionMiddleware(RequestDelegate next,
				ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
	{
		Next = next;
		Logger = logger;
		Env = env;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await Next(context);
		}
		catch (Exception ex)
		{
			int statusCode = (int)HttpStatusCode.InternalServerError;
			Logger.LogError(ex, ex.Message);
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = statusCode;

			ApiException response = Env.IsDevelopment()
					? new ApiException(statusCode, ex.Message, ex.StackTrace.ToString())
					: new ApiException(statusCode);

			JsonSerializerOptions options = new()
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			string json = JsonSerializer.Serialize(response, options);
			await context.Response.WriteAsync(json);
		}
	}
}
