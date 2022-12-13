using Core.Interfaces;
using CORE.Entities;
using Infrastructure.Data;
namespace Infrastructure.Repositories;

public class RolRepository : GenericRepository<Rol>, IRolRepository
{
    public RolRepository(TiendaContext context) : base(context)
    {
    }
}
