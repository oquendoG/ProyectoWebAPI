using Microsoft.EntityFrameworkCore;
using CORE.Entities;
using System.Reflection;

namespace Infrastructure.Data;
public class TiendaContext : DbContext
{
	public TiendaContext(DbContextOptions options) : base(options)
	{

	}

	public DbSet<Producto> Productos { get; set; }
	public DbSet<Marca> Marcas { get; set; }
	public DbSet<Categoria> Categorias { get; set; }
	public DbSet<Usuario> Usuarios { get; set; }
	public DbSet<Rol> Roles { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}
