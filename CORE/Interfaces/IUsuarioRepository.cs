using CORE.Entities;
namespace CORE.Interfaces;

public interface IUsuarioRepository : IGenericRepository<Usuario> {
    Task<Usuario> GetByUsernameAsync(string username);
}