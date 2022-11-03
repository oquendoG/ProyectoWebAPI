using CORE.Entities;
using CORE.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;
public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
{
	public CategoriaRepository(TiendaContext context) : base(context)
	{

	}
}
