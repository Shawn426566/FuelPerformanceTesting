namespace FuelApp.DTOs.Common
{
    /// <summary>
    /// Generic wrapper for paged API responses.
    /// </summary>
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        // Guard against division by zero when a malformed page size reaches this DTO.
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    }
}
