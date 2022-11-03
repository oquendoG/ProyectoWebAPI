namespace CORE.Interfaces;
public interface IUnitOfWork
{
    IProductoRepository Productos { get; }
    IMarcaRepository Marcas { get; }
    ICategoriaRepository Categorias { get; }

    void Dispose();
    int Save();
}
