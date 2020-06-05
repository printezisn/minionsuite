using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinionSuite.Tests.Models;

namespace MinionSuite.Tests.Templates
{
    public interface IPostService
    {
        Task<ResultModel<Post>> CreateAsync(Post entity);
        Task<bool> DeleteAsync(int key);
        Task<Post> GetAsync(int key);
        Task<List<Post>> GetAllAsync();
        Task<PageModel<Post>> GetAllAsync(int page, int pageSize, string sortField, bool asc);
        Task<List<Post>> SearchAsync(string term);
        Task<PageModel<Post>> SearchAsync(string term, int page, int pageSize, string sortField, bool asc);
        Task<ResultModel<Post>> UpdateAsync(Post entity);
    }
}
