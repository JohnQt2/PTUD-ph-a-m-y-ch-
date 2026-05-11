namespace MaterialApi.Models.Common;

/// <summary>
/// Model dùng để trả về kết quả có phân trang (pagination).
/// </summary>
public class PagedResult<T>
{
    /// <summary>Danh sách dữ liệu của trang hiện tại.</summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>Tổng số bản ghi (không phân trang).</summary>
    public int TotalCount { get; set; }

    /// <summary>Trang hiện tại (bắt đầu từ 1).</summary>
    public int Page { get; set; }

    /// <summary>Số bản ghi mỗi trang.</summary>
    public int PageSize { get; set; }

    /// <summary>Tổng số trang.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>Có trang trước không.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Có trang sau không.</summary>
    public bool HasNextPage => Page < TotalPages;

    public static PagedResult<T> Create(IEnumerable<T> source, int page, int pageSize)
    {
        var totalCount = source.Count();
        var items = source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
