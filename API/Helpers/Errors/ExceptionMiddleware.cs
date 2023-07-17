using System.Net;
using System.Text.Json;

namespace API.Helpers.Errors;

public class ExceptionMiddleware
{
	private RequestDelegate _next { get; }
	private ILogger<ExceptionMiddleware> _logger { get; }
	private IHostEnvironment _env { get; }

	public ExceptionMiddleware(RequestDelegate next,
				ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
	{
		_next = next;
		_logger = logger;
		_env = env;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			int statusCode = (int)HttpStatusCode.InternalServerError;
			_logger.LogError(ex, ex.Message);
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = statusCode;

			ApiException response = _env.IsDevelopment()
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
