namespace Chipseky.MamkinInvestor.WebApi.Dtos.Paging;

public class PagedList<T>
{
    private readonly IReadOnlyCollection<T> _items;

    public PagedList(IReadOnlyCollection<T> items, int page, int pageSize, long totalCount)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException(nameof(page));
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (totalCount < 0)
            throw new ArgumentOutOfRangeException(nameof(totalCount));

        _items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    public int Count => _items.Count;
    public int Page { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
}