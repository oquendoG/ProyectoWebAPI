using API.Dtos;
using API.Helpers;
using CORE.Entities;
using CORE.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace API.Services;

public class UserService : IUserService
{
    private readonly JWT jwt;
    private readonly IUnitOfWork unitOfWork;
    private readonly IPasswordHasher<Usuario> passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IOptions<JWT> jwt,
        IPasswordHasher<Usuario> passwordHasher)
    {
        this.jwt = jwt.Value;
        this.unitOfWork = unitOfWork;
        this.passwordHasher = passwordHasher;
    }

    public async Task<string> RegisterAsync(RegisterDTO registerDto)
    {
        var usuario = new Usuario
        {
            Nombre = registerDto.Nombres,
            ApellidoMaterno = registerDto.ApellidoMaterno,
            ApellidoPaterno = registerDto.ApellidoPaterno,
            Email = registerDto.Email,
            UserName = registerDto.Username
        };

        usuario.Password = passwordHasher.HashPassword(usuario, registerDto.Password);

        Usuario usuarioExiste = unitOfWork.Usuarios
                                    .Find(u => u.UserName.ToLower() == registerDto.Username.ToLower())
                                    .FirstOrDefault();

        if (usuarioExiste == null)
        {
            Rol rolPredeterminado = unitOfWork.Roles
                                    .Find(u => u.Nombre == Autorizacion.rol_predeterminado.ToString())
                                    .First();
            try
            {
                usuario.Roles.Add(rolPredeterminado);
                unitOfWork.Usuarios.Add(usuario);
                await unitOfWork.SaveAsync();

                return $"El usuario  {registerDto.Username } ha sido registrado exitosamente";
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return $"Error: {message}";
            }
        }
        else
        {
            return $"El usuario con {registerDto.Username } ya se encuentra registrado.";
        }
    }


    public async Task<DatosUsuarioDto> GetTokenAsync(LoginDTO model)
    {
        DatosUsuarioDto datosUsuarioDto = new();
        Usuario usuario = await unitOfWork.Usuarios
                    .GetByUsernameAsync(model.Username);

        if (usuario is null)
        {
            datosUsuarioDto.EstaAutenticado = false;
            datosUsuarioDto.Mensaje = $"No existe ningún usuario con el username {model.Username}.";
            return datosUsuarioDto;
        }

        PasswordVerificationResult resultado =
            passwordHasher
            .VerifyHashedPassword(usuario, usuario.Password, model.Password);

        if (resultado == PasswordVerificationResult.Success)
        {
            datosUsuarioDto.EstaAutenticado = true;
            JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
            datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            datosUsuarioDto.Email = usuario.Email;
            datosUsuarioDto.UserName = usuario.UserName;
            datosUsuarioDto.Roles = usuario.Roles
                                            .Select(u => u.Nombre)
                                            .ToList();
            return datosUsuarioDto;
        }
        datosUsuarioDto.EstaAutenticado = false;
        datosUsuarioDto.Mensaje = $"Credenciales incorrectas para el usuario {usuario.UserName}.";
        return datosUsuarioDto;
    }

    public async Task<string> AddRoleAsync(AddRoleDto model)
    {

        Usuario usuario = await unitOfWork.Usuarios
                    .GetByUsernameAsync(model.Username);

        if (usuario == null)
        {
            return $"No existe algún usuario registrado con la cuenta {model.Username}.";
        }


        PasswordVerificationResult resultado =
            passwordHasher
            .VerifyHashedPassword(usuario, usuario.Password, model.Password);

        if (resultado != PasswordVerificationResult.Success)
        {
            return $"Credenciales incorrectas para el usuario {usuario.UserName}.";
        }

        Rol rolExiste = unitOfWork.Roles
                        .Find(rol => rol.Nombre.ToLower() == model.Role.ToLower())
                        .FirstOrDefault();

        if (rolExiste is null)
        {
            return $"Rol {model.Role} no encontrado.";
        }

        bool usuarioTieneRol = usuario.Roles.Any(rol => rol.Id == rolExiste.Id);

        if (usuarioTieneRol is false)
        {
            usuario.Roles.Add(rolExiste);
            unitOfWork.Usuarios.Update(usuario);
            await unitOfWork.SaveAsync();
        }

        return $"Rol {model.Role} agregado a la cuenta {model.Username} de forma exitosa.";
    }

    private JwtSecurityToken CreateJwtToken(Usuario usuario)
    {
        ICollection<Rol> roles = usuario.Roles;
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role.Nombre));
        }
        IEnumerable<Claim> claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim("uid", usuario.Id.ToString())
        }
        .Union(roleClaims);
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

}
