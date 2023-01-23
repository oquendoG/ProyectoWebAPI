namespace API.Helpers;

public class Pager<T> where T : class
{
    /// <summary>
    /// P�gina actual
    /// </summary>
    public int PageIndex { get; private set; }

    /// <summary>
    /// Cantidad de registros por p�gina
    /// </summary>
    public int PageSize{ get; private set; }

    /// <summary>
    /// Cantidad de registros
    /// </summary>
    public int Total{ get; private set; }

    /// <summary>
    /// Los registros que llevar� la p�gina
    /// </summary>
    public IEnumerable<T> Registers { get; private set; }

    public string Search { get; private set; }

    public Pager(int pageIndex, int pageSize, int total, IEnumerable<T> registers, string search)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Total = total;
        Registers = registers;
        Search = search;
    }

    /// <summary>
    /// Cantidad de paginas seg�n la cantidad de registros
    /// </summary>
    public int TotalPages
    {
        get
        {
            return (int)Math.Ceiling(Total / (double)PageSize);
        }
    }

    public bool HasPreviousPage
    {
        get
        {
            return PageIndex > 1;
        }
    }

    public bool HasNextPage
    {
        get
        {
            return PageIndex < TotalPages;
        }
    }
}