using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MinionSuite.Tests.Templates
{
    public class PageModel<T> : IPageModel, IEnumerable<T>
    {
        public IEnumerable<T> Collection { get; private set; }
        public int TotalItems { get; private set; }
        public int TotalPages { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public bool IsFirstPage => Page == 1;
        public bool IsLastPage => Page == TotalPages;

        private PageModel()
        {
        }

        public static async Task<PageModel<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
        {
            var pageModel = new PageModel<T>();

            pageModel.PageSize = pageSize;
            pageModel.TotalItems = query.Count();
            pageModel.TotalPages = (int)Math.Ceiling(pageModel.TotalItems / (double)pageModel.PageSize);
            pageModel.Page = Math.Max(1, page);
            pageModel.Page = Math.Min(pageModel.Page, pageModel.TotalPages);
            pageModel.Collection = await query.Skip((pageModel.Page - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToListAsync();

            return pageModel;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }
    }
}
