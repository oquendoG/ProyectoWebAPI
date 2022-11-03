using CORE.Entities;
using CORE.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;
public class MarcaRepository : GenericRepository<Marca>, IMarcaRepository
{
	public MarcaRepository(TiendaContext context) : base(context)
	{

	}
}
