namespace API.Helpers;

public class Params
{
    private int pageSize = 5;
    private const int MaxPageSize = 50;
    private int pageIndex = 1;
    private string search;

    public int PageSize
    {
        get => pageSize;
        set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public int PageIndex
    {
        get => pageIndex;
        set => pageIndex = (value <= 0) ? 1 : value;
    }

    public string Search
    {
        get => search;
        set => search = (!String.IsNullOrEmpty(value)) ? value.ToLower() : "";
    }
}
