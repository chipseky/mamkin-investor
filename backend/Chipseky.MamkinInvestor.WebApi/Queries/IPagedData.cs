namespace Chipseky.MamkinInvestor.WebApi.Queries;

public interface IPagedData<T>
{
    int Page { get; }
    int PageSize { get; }
    long TotalCount { get; }
    public int TotalPages => (int)((TotalCount - 1) / PageSize + 1);
    public IReadOnlyCollection<T> Items { get; }
}