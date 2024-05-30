using API.Dtos;
using API.Helpers;
using CORE.Entities;
using CORE.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace API.Services;

public class UserService : IUserService
{
    private readonly JWT _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IOptions<JWT> jwt,
                                               IPasswordHasher<Usuario> passwordHasher)
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Método que permite agregar un nuevo usuario a la aplicación web con un rol por defecto
    /// </summary>
    /// <param name="registerDto">Recive todos los datos del usuraio</param>
    /// <returns>Un mensaje de error o de exito si el registro fue o no correcto</returns>
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

        usuario.Password = _passwordHasher.HashPassword(usuario, registerDto.Password);

        Usuario usuarioDB = _unitOfWork.Usuarios
                    .Find(user => user.UserName.ToLower() == registerDto.Username.ToLower())
                    .FirstOrDefault();

        if (usuarioDB is not null)
        {
            return $"El usuario con {registerDto.Username} ya se encuentra registrado.";
        }

        Rol rolPredeterminado = _unitOfWork.Roles
                                    .Find(rol => rol.Nombre == Autorizacion.rol_predeterminado.ToString(), false)
                                    .FirstOrDefault();
        try
        {
            usuario.Roles.Add(rolPredeterminado);
            _unitOfWork.Usuarios.Add(usuario);
            await _unitOfWork.SaveAsync();

            return $"El usuario {registerDto.Username} ha sido registrado exitosamente";
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            return $"Error: {message}";
        }
    }

    /// <summary>
    /// Logea al usuario y genera un token (JWT)
    /// </summary>
    /// <param name="model">Recibe los datos de inicio de sesión del usuario por medio de un dto</param>
    /// <returns></returns>
    public async Task<DatosUsuarioDto> GetTokenAsync(LoginDTO model)
    {
        //Esta clase lleva los datos que se incluyen en el web token
        DatosUsuarioDto datosUsuarioDto = new();
        Usuario usuarioDB = await _unitOfWork.Usuarios
                                        .GetByUsernameAsync(model.Username);
        if (usuarioDB is null)
        {
            datosUsuarioDto.EstaAutenticado = false;
            datosUsuarioDto.Mensaje = $"No existe ningún usuario con el username {model.Username}.";
            return datosUsuarioDto;
        }

        //Validamos contraseña
        PasswordVerificationResult resultado =
                            _passwordHasher
                            .VerifyHashedPassword(usuarioDB, usuarioDB.Password, model.Password);

        if (resultado != PasswordVerificationResult.Success)
        {
            datosUsuarioDto.EstaAutenticado = false;
            datosUsuarioDto.Mensaje = $"Credenciales incorrectas para el usuario {usuarioDB.UserName}.";
            return datosUsuarioDto;
        }

        datosUsuarioDto.EstaAutenticado = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuarioDB);
        datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        datosUsuarioDto.Email = usuarioDB.Email;
        datosUsuarioDto.Username = usuarioDB.UserName;
        datosUsuarioDto.Roles = usuarioDB.Roles
                                        .Select(rol => rol.Nombre)
                                        .ToList();

        DatosUsuarioDto datosUsuarioDtoWithRefreshToken =
                        await AssingRefreshToken(datosUsuarioDto, usuarioDB);

        return datosUsuarioDtoWithRefreshToken;
    }

    /// <summary>
    /// Método usado para generar el JWT
    /// </summary>
    /// <param name="usuario">Recibe un objeto de Clase Core.Entities.Usuario</param>
    /// <returns>El JWT</returns>
    private JwtSecurityToken CreateJwtToken(Usuario usuario)
    {
        ICollection<Rol> roles = usuario.Roles;

        List<Claim> roleClaims = new();
        foreach (Rol role in roles)
        {
            roleClaims.Add(new Claim("roles", role.Nombre));
        }

        IEnumerable<Claim> claims = new[]
        {
            //Este es el id del jwt
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            //Identifica al sujeto de quien es el jwt
            new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),

            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim("uid", usuario.Id.ToString())
        }
        .Union(roleClaims);

        //Esta clase genera la firma del token y genera las credenciales de inicio de sesión
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    /// <summary>
    /// Asignamos un refresh token a un usuario determinado
    /// </summary>
    /// <param name="datosUsuarioDto">Un dto que recive los datos del usuario</param>
    /// <param name="usuario">Un objeto Usuario ya instanciado</param>
    private async Task<DatosUsuarioDto> AssingRefreshToken(DatosUsuarioDto datosUsuarioDto, Usuario usuario)
    {
        if (usuario.RefreshTokens.Any(refreshToken => refreshToken.IsActive))
        {
            RefreshToken activeRefreshToken =
                usuario.RefreshTokens.FirstOrDefault(refreshToken => refreshToken.IsActive);
            datosUsuarioDto.RefreshToken = activeRefreshToken.Token;
            datosUsuarioDto.RefreshTokenExpiration = activeRefreshToken.Expires;
            return datosUsuarioDto;
        }

        RefreshToken refreshToken = CreateRefreshToken();
        datosUsuarioDto.RefreshToken = refreshToken.Token;
        datosUsuarioDto.RefreshTokenExpiration = refreshToken.Expires;

        usuario.RefreshTokens.Add(refreshToken);
        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.SaveAsync();

        return datosUsuarioDto;
    }

    /// <summary>
    /// Permite agregar un nuevo rol al usuario
    /// </summary>
    /// <param name="model">Es un DTO con los datos de login del usuario</param>
    /// <returns>Un texto de validación</returns>
    public async Task<string> AddRoleAsync(AddRoleDto model)
    {
        Usuario usuario = await _unitOfWork.Usuarios.GetByUsernameAsync(model.Username);

        if (usuario == null)
        {
            return $"No existe algún usuario registrado con la cuenta {model.Username}.";
        }

        PasswordVerificationResult resultado =
                            _passwordHasher
                            .VerifyHashedPassword(usuario, usuario.Password, model.Password);

        if (resultado != PasswordVerificationResult.Success)
        {
            return $"Credenciales incorrectas para el usuario {usuario.UserName}.";
        }

        Rol rolExisteEnBd = _unitOfWork.Roles
                                .Find(rol => rol.Nombre.ToLower() == model.Role.ToLower())
                                .FirstOrDefault();

        if (rolExisteEnBd is null)
        {
            return $"Rol {model.Role} no encontrado.";
        }

        bool usuarioTieneRol = usuario.Roles.Any(rol => rol.Id == rolExisteEnBd.Id);

        if (usuarioTieneRol)
        {
            return $" El usuario ya tiene el Rol {model.Role} asignado";
        }

        usuario.Roles.Add(rolExisteEnBd);
        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.SaveAsync();

        return $"Rol {model.Role} agregado a la cuenta {model.Username} de forma exitosa.";
    }

    /// <summary>
    /// Nos permite crear un nuevo refresh token para el Usuario
    /// </summary>
    /// <returns>Una instancia de Core.Entities.RefreshToken</returns>
    private static RefreshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            Expires = DateTime.UtcNow.AddDays(10),
            Created = DateTime.UtcNow
        };
    }

    public async Task<DatosUsuarioDto> RefreshTokenAsync(string refreshToken)
    {
        DatosUsuarioDto datosUsuarioDto = new();
        Usuario usuario = await _unitOfWork.Usuarios.GetByRefreshTokenAsync(refreshToken);
        if (usuario is null)
        {
            datosUsuarioDto.EstaAutenticado = false;
            datosUsuarioDto.Mensaje = "El token no pertenece a ningún usuario.";
            return datosUsuarioDto;
        }

        RefreshToken refreshTokenBd = usuario.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken);
        if (!refreshTokenBd.IsActive)
        {
            datosUsuarioDto.EstaAutenticado = false;
            datosUsuarioDto.Mensaje = "El token no está activo.";
            return datosUsuarioDto;
        }

        //Revocamos el Refresh Token actual y
        refreshTokenBd.Revoked = DateTime.UtcNow;

        //generamos un nuevo Refresh Token y lo guardamos en la Base de Datos
        RefreshToken newRefreshToken = CreateRefreshToken();
        usuario.RefreshTokens.Add(newRefreshToken);
        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.SaveAsync();

        //Generamos un nuevo Json Web Token
        datosUsuarioDto.EstaAutenticado = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);

        //Cargamos el dto
        datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        datosUsuarioDto.Email = usuario.Email;
        datosUsuarioDto.Username = usuario.UserName;
        datosUsuarioDto.Roles = usuario.Roles.Select(rol => rol.Nombre).ToList();
        datosUsuarioDto.RefreshToken = newRefreshToken.Token;
        datosUsuarioDto.RefreshTokenExpiration = newRefreshToken.Expires;
        return datosUsuarioDto;
    }
}
