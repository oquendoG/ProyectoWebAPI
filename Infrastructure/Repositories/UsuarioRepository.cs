using CORE.Entities;
using CORE.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(TiendaContext context) : base(context)
    {
    }

    public async Task<Usuario> GetByUsernameAsync(string username)
    {
        return await context.Usuarios
                .Include(u=>u.Roles)
                .Include(u=>u.RefreshTokens)
                .FirstOrDefaultAsync(u=>u.UserName.ToLower()==username.ToLower());
    }

    public async Task<Usuario> GetByRefreshTokenAsync(string refreshToken)
    {
        return await context.Usuarios
                 .Include(u => u.Roles)
                 .Include(u => u.RefreshTokens)
                 .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token==refreshToken));
    }
}