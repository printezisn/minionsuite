using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinionSuiteExample.Web.Models;

namespace MinionSuiteExample.Web.Services
{
    public interface IPostService
    {
        Task<ResultModel<Post>> CreateAsync(Post entity);
        Task DeleteAsync(Post entity);
        Task<Post> GetAsync(int key);
        Task<List<Post>> GetAllAsync();
        Task<PageModel<Post>> GetAllAsync(int page, int pageSize, string sortField, bool asc);
        Task<List<Post>> SearchAsync(string term);
        Task<PageModel<Post>> SearchAsync(string term, int page, int pageSize, string sortField, bool asc);
        Task<ResultModel<Post>> UpdateAsync(Post entity);
    }
}
