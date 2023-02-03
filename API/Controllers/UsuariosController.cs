using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UsuariosController : BaseApiController
{
    private readonly IUserService _userService;

    public UsuariosController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Este método registra el nuevo usario usando el método de registro que está
    /// en el servicio de usuarios
    /// </summary>
    /// <param name="model">Es un DTO que recibe los datos enviados por el usuario</param>
    /// <returns>Un resultado de satisfacción al regidtrar el usuario</returns>
    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterDTO model)
    {
        string result = await _userService.RegisterAsync(model);
        return Ok(result);
    }

    /// <summary>
    /// Permite logear al usuario y generar el JWT usando el objeto API.Services.UserService
    /// </summary>
    /// <param name="model">Recive un objeto LoginDTO desde el cuerpo de la petición</param>
    /// <returns></returns>
    [HttpPost("token")]
    public async Task<IActionResult> GetTokenAsync(LoginDTO model)
    {
        DatosUsuarioDto result = await _userService.GetTokenAsync(model);
        SetRefreshTokenInCookie(result.RefreshToken);
        return Ok(result);
    }

    [HttpPost("addrole")]
    public async Task<IActionResult> AddRoleAsync(AddRoleDto model)
    {
        string result = await _userService.AddRoleAsync(model);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        string refreshToken = Request.Cookies["refreshToken"];
        DatosUsuarioDto response = await _userService.RefreshTokenAsync(refreshToken);

        //Si es valido se pone nuevamente en la cookie
        if (!string.IsNullOrEmpty(response.RefreshToken))
            SetRefreshTokenInCookie(response.RefreshToken);
        return Ok(response);
    }

    private void SetRefreshTokenInCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(10),
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
