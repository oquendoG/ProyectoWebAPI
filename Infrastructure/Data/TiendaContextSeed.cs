using System.Globalization;
using System.Reflection;
using CORE.Entities;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

/// <summary>
/// Permite agregar información por defecto a la base de datos
/// </summary>
public class TiendaContextSeed
{
    /// <summary>
    /// Este método agrega informaciónpor defecto a las tablas
    /// </summary>
    /// <param name="context">Este parámetro es la clase de contexto de EF</param>
    /// <param name="loggerFactory">Este parámetro es para hacer logging</param>
    /// <returns>Un tarea</returns>
    public static async Task SeedAsync(TiendaContext context,
    ILoggerFactory loggerFactory)
    {
        try
        {
            string ruta = Path.GetDirectoryName(Assembly.GetExecutingAssembly()
            .Location);

            if(!context.Marcas.Any()){

                using var readerMarcas = new StreamReader(ruta+"/Data/CSVS/marcas.csv");

                using var csvMarcas = new CsvReader(readerMarcas, CultureInfo
                .InvariantCulture);

                IEnumerable<Marca> marcas = csvMarcas.GetRecords<Marca>();
                context.Marcas.AddRange(marcas);
                await context.SaveChangesAsync();
            }

            if(!context.Categorias.Any()){

                using var readerCategorias = new StreamReader(ruta+"/Data/CSVS/categorias.csv");

                using var csvCategorias = new CsvReader(readerCategorias, CultureInfo.InvariantCulture);

                IEnumerable<Categoria> categorias = csvCategorias.GetRecords<Categoria>();
                context.Categorias.AddRange(categorias);
                await context.SaveChangesAsync();
            }

            if(!context.Productos.Any()){

                using var readerProductos = new StreamReader(ruta+"/Data/CSVS/productos.csv");

                using var csvProductos = new CsvReader(readerProductos, CultureInfo.InvariantCulture);

                IEnumerable<Producto> ListaproductosCsv = csvProductos.GetRecords<Producto>();

                List<Producto> productos = new();
                foreach(var item in ListaproductosCsv){
                    productos.Add(
                        new Producto{
                            Id = item.Id,
                            Nombre = item.Nombre,
                            Precio = item.Precio,
                            FechaCreacion = item.FechaCreacion,
                            CategoriaId = item.CategoriaId,
                            MarcaId = item.MarcaId
                    });
                }
                context.Productos.AddRange(productos);
                await context.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            ILogger<TiendaContextSeed> logger = loggerFactory.CreateLogger<TiendaContextSeed>();
            logger.LogError(ex.Message);
        }
    }

    /// <summary>
    /// Este método agrega roles por defecto a la base de datos
    /// </summary>
    /// <param name="context">Este parámetro es la clase de contexto de EF</param>
    /// <param name="loggerFactory">Este parámetro es para hacer logging</param>
    /// <returns>una tarea</returns>
    public static async Task SeedRolesAsync(TiendaContext context, ILoggerFactory loggerFactory)
    {
        try
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Rol>()
                {
                    new Rol{Id=1, Nombre="Administrador"},
                    new Rol{Id=2, Nombre="Gerente"},
                    new Rol{Id=3, Nombre="Empleado"},
                };
                context.Roles.AddRange(roles);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<TiendaContextSeed>();
            logger.LogError(message: ex.Message);
        }
    }
}