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
        return await _context.Usuarios
                        .Include(user=>user.Roles)
                        .Include(user=>user.RefreshTokens)
                        .FirstOrDefaultAsync(user=>user.UserName.ToLower()==username.ToLower());
    }

    public async Task<Usuario> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Usuarios
                 .Include(user => user.Roles)
                 .Include(user => user.RefreshTokens)
                 .FirstOrDefaultAsync(user => user.RefreshTokens.Any(rt => rt.Token==refreshToken));
    }
}