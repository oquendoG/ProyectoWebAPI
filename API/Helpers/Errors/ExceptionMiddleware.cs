using System.Net;
using System.Text.Json;

namespace API.Helpers.Errors;

public class ExceptionMiddleware
{
	private readonly RequestDelegate next;
	private readonly ILogger<ExceptionMiddleware> logger;
	private readonly IHostEnvironment env;

	public ExceptionMiddleware(RequestDelegate next,
				ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
	{
		this.next = next;
		this.logger = logger;
		this.env = env;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
			int statusCode = (int)HttpStatusCode.InternalServerError;
			logger.LogError(ex, message: ex.Message);
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = statusCode;

			ApiException response = env.IsDevelopment()
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
