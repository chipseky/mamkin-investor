namespace Chipseky.MamkinInvestor.WebApi.Dtos;

public interface IPagination
{
    int Page { get; set; }
    int PageSize { get; set; }
}