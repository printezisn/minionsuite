using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinionSuiteExample.Web.Data;
using MinionSuiteExample.Web.Models;

namespace MinionSuiteExample.Web.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<ResultModel<Comment>> CreateAsync(Comment model)
        {
            if (!await _context.Posts.AnyAsync(a => a.Id == model.PostId))
            {
                return new ResultModel<Comment>("The post was not found.");
            }

            var newEntity = new Comment();

            newEntity.Body = model.Body;
            newEntity.PostId = model.PostId;
            newEntity.CreatedAt = DateTime.UtcNow;
            newEntity.UpdatedAt = DateTime.UtcNow;

            _context.Comments.Add(newEntity);
            await _context.SaveChangesAsync();

            return new ResultModel<Comment>(newEntity);
        }

        public virtual async Task<bool> DeleteAsync(int key)
        {
            var entity = await GetAsync(key);
            if (entity == null)
            {
                return false;
            }

            _context.Comments.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public virtual Task<Comment> GetAsync(int key)
        {
            return _context.Comments.FirstOrDefaultAsync(f => f.Id == key);
        }

        public virtual Task<List<Comment>> GetAllAsync(int postId)
        {
            return _context.Comments.AsNoTracking().Where(w => w.PostId == postId).ToListAsync();
        }

        public virtual Task<PageModel<Comment>> GetAllAsync(int postId, int page, int pageSize, string sortField, bool asc)
        {
            var query = _context.Comments.AsNoTracking().Where(w => w.PostId == postId);
            query = GetSortedQuery(query, sortField, asc);

            return PageModel<Comment>.CreateAsync(query, page, pageSize, sortField, asc);
        }

        public virtual Task<List<Comment>> SearchAsync(int postId, string term)
        {
            var query = _context.Comments.AsNoTracking().Where(w => w.PostId == postId);
            return query.Where(w => w.Body.Contains(term)).ToListAsync();
        }

        public virtual Task<PageModel<Comment>> SearchAsync(int postId, string term, int page, int pageSize, string sortField, bool asc)
        {
            var query = _context.Comments.AsNoTracking().Where(w => w.PostId == postId);
            query = query.Where(w => w.Body.Contains(term));
            query = GetSortedQuery(query, sortField, asc);

            return PageModel<Comment>.CreateAsync(query, page, pageSize, sortField, asc);
        }

        public virtual async Task<ResultModel<Comment>> UpdateAsync(Comment model)
        {
            var existingEntity = await GetAsync(model.Id);
            if (existingEntity == null)
            {
                return null;
            }

            existingEntity.Body = model.Body;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResultModel<Comment>(existingEntity);
        }

        protected virtual IQueryable<Comment> GetSortedQuery(IQueryable<Comment> query, string sortField, bool asc)
        {
            switch (sortField)
            {
                case "CreatedAt":
                    return asc
                        ? query
                            .OrderBy(o => o.CreatedAt)
                            .ThenBy(o => o.Id)
                        : query
                            .OrderByDescending(o => o.CreatedAt)
                            .ThenBy(o => o.Id);
                case "UpdatedAt":
                    return asc
                        ? query
                            .OrderBy(o => o.UpdatedAt)
                            .ThenBy(o => o.Id)
                        : query
                            .OrderByDescending(o => o.UpdatedAt)
                            .ThenBy(o => o.Id);
                default:
                    return asc
                        ? query.OrderBy(o => o.Id)
                        : query.OrderByDescending(o => o.Id);
            }
        }
    }
}
