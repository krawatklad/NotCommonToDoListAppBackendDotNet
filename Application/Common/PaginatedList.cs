namespace Application.Common;

public record PaginatedList<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int TotalCount,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

