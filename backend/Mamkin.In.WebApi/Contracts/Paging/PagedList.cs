namespace Mamkin.In.WebApi.Contracts.Paging;

public class PagedData<T> : IPagedData<T>
{
    public PagedData(IReadOnlyCollection<T> items, int page, int pageSize, long totalCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
        ArgumentOutOfRangeException.ThrowIfNegative(totalCount);

        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public int Page { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
    
    public IReadOnlyCollection<T> Items { get; }
}