namespace Mamkin.In.WebApi.Contracts.Paging;

public interface IPagination
{
    int Page { get; set; }
    int PageSize { get; set; }
}