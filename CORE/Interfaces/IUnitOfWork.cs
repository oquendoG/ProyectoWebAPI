using Core.Interfaces;

namespace CORE.Interfaces;
public interface IUnitOfWork
{
    IProductoRepository Productos { get; }
    IMarcaRepository Marcas { get; }
    ICategoriaRepository Categorias { get; }
    IRolRepository Roles { get; }
    IUsuarioRepository Usuarios { get; }
    void Dispose();
    Task<int> SaveAsync();
}
