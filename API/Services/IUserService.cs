using API.Dtos;

namespace API.Services;
public interface IUserService
{
    Task<string> RegisterAsync(RegisterDTO model);
    Task<DatosUsuarioDto> GetTokenAsync(LoginDTO model);
    Task<string> AddRoleAsync(AddRoleDto model);
}
