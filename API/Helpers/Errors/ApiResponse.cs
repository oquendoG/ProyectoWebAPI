namespace API.Helpers.Errors;

/// <summary>
/// Esta clase se escarga de generar mensajes de error standard para la API
/// </summary>
public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public ApiResponse(int statusCode, string message = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessage(statusCode);
    }

    /// <summary>
    /// Si el desarrollador no envía un mensaje de error este método devuelve uno por defecto
    /// </summary>
    /// <param name="statusCode">Código de error como 400, 404</param>
    /// <returns>Un mensaje de error</returns>
    private static string GetDefaultMessage(int statusCode)
    {
        return statusCode switch
        {
            400 => "Has realizado una petición incorrecta",
            401 => "Usuario no autorizado",
            404 => "El recurso no existe",
            405 => "Este método no estápermitido en el servidor",
            500 => "Error del servidor, por favor comunicate con el administrador",
            _ => "Error desconocido"
        };
    }
}
