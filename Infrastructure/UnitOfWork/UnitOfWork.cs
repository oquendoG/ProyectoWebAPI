using Core.Interfaces;
using CORE.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitOfWork;
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly TiendaContext _context;
    private IProductoRepository _productos;
    private IMarcaRepository _marcas;
    private ICategoriaRepository _categorias;
    private IUsuarioRepository _usuarios;
    private IRolRepository _roles;

    public UnitOfWork(TiendaContext context)
    {
        _context = context;
    }

    public IProductoRepository Productos
    {
        get
        {
            if (_productos is null)
            {
                _productos = new ProductoRepository(_context);
            }

            return _productos;
        }
    }

    public IMarcaRepository Marcas
    {
        get
        {
            if (_marcas is null)
            {
                _marcas = new MarcaRepository(_context);
            }

            return _marcas;
        }
    }

    public ICategoriaRepository Categorias
    {
        get
        {
            if (_categorias is null)
            {
                _categorias = new CategoriaRepository(_context);
            }

            return _categorias;
        }
    }

    public IRolRepository Roles
    {
        get
        {
            if (_roles == null)
            {
                _roles = new RolRepository(_context);
            }
            return _roles;
        }
    }

    public IUsuarioRepository Usuarios
    {
        get
        {
            if (_usuarios == null)
            {
                _usuarios = new UsuarioRepository(_context);
            }
            return _usuarios;
        }
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
