using CORE.Entities;
using CORE.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class ProductoRepository : GenericRepository<Producto>,
    IProductoRepository
{
	public ProductoRepository(TiendaContext context) : base(context)
	{

	}

	public async Task<IEnumerable<Producto>>
		GetProductosMasCaros(int cantidad)
	{
		return await this.context.Productos
		.OrderByDescending(p => p.Precio)
		.Take(cantidad)
		.ToListAsync();
	}
}
