using CORE.Entities;

namespace CORE.Interfaces;
public interface IProductoRepository : IGenericRepository<Producto>
{
    Task<Producto> GetByIdAsync(int id);
    Task<IEnumerable<Producto>> GetProductosMasCaros(int cantidad);
}
