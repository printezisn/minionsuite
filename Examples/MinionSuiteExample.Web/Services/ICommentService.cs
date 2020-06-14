using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinionSuiteExample.Web.Models;

namespace MinionSuiteExample.Web.Services
{
    public interface ICommentService
    {
        Task<ResultModel<Comment>> CreateAsync(Comment entity);
        Task<bool> DeleteAsync(int key);
        Task<Comment> GetAsync(int key);
        Task<List<Comment>> GetAllAsync(int postId);
        Task<PageModel<Comment>> GetAllAsync(int postId, int page, int pageSize, string sortField, bool asc);
        Task<List<Comment>> SearchAsync(int postId, string term);
        Task<PageModel<Comment>> SearchAsync(int postId, string term, int page, int pageSize, string sortField, bool asc);
        Task<ResultModel<Comment>> UpdateAsync(Comment entity);
    }
}
