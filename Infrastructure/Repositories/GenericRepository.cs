using CORE.Entities;
using CORE.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly TiendaContext _context;

    public GenericRepository(TiendaContext context)
    {
        _context = context;
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }

    public IEnumerable<T> Find(Expression<Func<T, bool>> expression, bool noTracking = true)
    {
        return noTracking ? _context.Set<T>().AsNoTracking().Where(expression)
                          : _context.Set<T>().Where(expression);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(bool noTracking = true)
    {
        return noTracking ? await _context.Set<T>().AsNoTracking().ToListAsync()
                          : await _context.Set<T>().ToListAsync();
    }

    public virtual async Task<(int totalRegistros, IEnumerable<T> registros)>
                GetAllAsync(int pageIndex, int pageSize, string search, bool noTracking = true)
    {
        var query = noTracking ? _context.Set<T>().AsNoTracking().AsQueryable()
                               : _context.Set<T>().AsQueryable();

        int totalRegistros = await _context.Set<T>().CountAsync();
        List<T> registros = await query
                                    .Skip((pageIndex - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

        return (totalRegistros, registros);
    }

    public virtual async Task<T> GetByIdAsync(int id, bool noTracking = true)
    {
        T entity = await _context.Set<T>().FindAsync(id);
        if (noTracking)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
}
