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
                .FirstOrDefaultAsync(u=>u.UserName.ToLower()==username.ToLower());
    }
}