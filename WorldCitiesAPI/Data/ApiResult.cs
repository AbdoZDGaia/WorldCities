using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace WorldCitiesAPI.Data
{
    public class ApiResult<T>
    {
        public List<T> Data { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public string? SortColum { get; set; }
        public string? SortOrder { get; set; }
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }
        public bool HasNextPage
        {
            get { return ((PageIndex + 1) < TotalPages); }
        }
        private ApiResult(List<T> data, int count, int pageIndex, int pageSize, string? sortColum, string? sortOrder)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColum = sortColum;
            SortOrder = sortOrder;
        }

        public static async Task<ApiResult<T>> CreateAsync(
            IQueryable<T> source,
            int pageIndex,
            int pageSize,
            string? sortColumn = null,
            string? sortOrder = null)
        {
            var count = await source.CountAsync();

            if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
            {
                sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC" ?
                    "ASC" : "DESC";
                source = source.OrderBy(string.Format("{0} {1}", sortColumn, sortOrder));
            }

            source = source
                .Skip(pageIndex * pageSize)
                .Take(pageSize);

            var data = await source.ToListAsync();

            var result = new ApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder);
            return result;
        }

        public static bool IsValidProperty(string propertyName,
            bool throwExceptionIfNotFound = true)
        {
            var prop = typeof(T).GetProperty(
                propertyName,
                BindingFlags.IgnoreCase |
                BindingFlags.Public |
                BindingFlags.Instance);

            if (prop == null && throwExceptionIfNotFound)
                throw new NotSupportedException(string.Format($"ERROR: Property{propertyName} does not exist."));

            return prop != null;
        }
    }
}
