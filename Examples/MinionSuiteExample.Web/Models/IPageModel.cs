using System;

namespace MinionSuiteExample.Web.Models
{
    public interface IPageModel
    {
        int TotalItems { get; }
        int TotalPages { get; }
        int Page { get; }
        int PageSize { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
        string SortField { get; }
        bool IsAscending { get; }
    }
}
