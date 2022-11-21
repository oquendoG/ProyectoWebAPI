using CORE.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork;
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly TiendaContext context;
    private IProductoRepository productos;
    private IMarcaRepository marcas;
    private ICategoriaRepository categorias;

    public UnitOfWork(TiendaContext context)
    {
        this.context = context;
    }

    public IProductoRepository Productos
    {
        get
        {
            if (productos is null)
            {
                productos = new ProductoRepository(context);
            }

            return productos;
        }
    }

    public IMarcaRepository Marcas
    {
        get
        {
            if (marcas is null)
            {
                marcas = new MarcaRepository(context);
            }

            return marcas;
        }
    }

    public ICategoriaRepository Categorias
    {
        get
        {
            if (categorias is null)
            {
                categorias = new CategoriaRepository(context);
            }

            return categorias;
        }
    }

    public async Task<int> SaveAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}
