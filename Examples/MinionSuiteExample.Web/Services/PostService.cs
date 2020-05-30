using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinionSuiteExample.Web.Data;
using MinionSuiteExample.Web.Models;

namespace MinionSuiteExample.Web.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<ResultModel<Post>> CreateAsync(Post entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Posts.Add(entity);
            await _context.SaveChangesAsync();

            return new ResultModel<Post>(entity);
        }

        public virtual Task DeleteAsync(Post entity)
        {
            _context.Posts.Remove(entity);
            return _context.SaveChangesAsync();
        }

        public virtual Task<Post> GetAsync(int key)
        {
            return _context.Posts.FindAsync(key).AsTask();
        }

        public virtual Task<List<Post>> GetAllAsync()
        {
            return _context.Posts.ToListAsync();
        }

        public virtual Task<PageModel<Post>> GetAllAsync(int page, int pageSize, string sortField, bool asc)
        {
            var query = _context.Posts.AsNoTracking();
            query = GetSortedQuery(query, sortField, asc);

            return PageModel<Post>.CreateAsync(query, page, pageSize, sortField, asc);
        }

        public virtual Task<List<Post>> SearchAsync(string term)
        {
            return _context.Posts.Where(w => w.Title.Contains(term) || w.Body.Contains(term)).ToListAsync();
        }

        public virtual Task<PageModel<Post>> SearchAsync(string term, int page, int pageSize, string sortField, bool asc)
        {
            var query = _context.Posts.AsNoTracking().Where(w => w.Title.Contains(term) || w.Body.Contains(term));
            query = GetSortedQuery(query, sortField, asc);

            return PageModel<Post>.CreateAsync(query, page, pageSize, sortField, asc);
        }

        public virtual async Task<ResultModel<Post>> UpdateAsync(Post entity)
        {
            var existingEntity = await _context.Posts.FindAsync(entity.Id).AsTask();
            if (existingEntity == null)
            {
                return new ResultModel<Post>("The entity was not found.");
            }

            existingEntity.Title = entity.Title;
            existingEntity.Body = entity.Body;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResultModel<Post>(existingEntity);
        }

        protected virtual IQueryable<Post> GetSortedQuery(IQueryable<Post> query, string sortField, bool asc)
        {
            switch (sortField)
            {
                case "Title":
                    return asc
                        ? query
                            .OrderBy(o => o.Title)
                            .ThenBy(o => o.Id)
                        : query
                            .OrderByDescending(o => o.Title)
                            .ThenBy(o => o.Id);
                case "Body":
                    return asc
                        ? query
                            .OrderBy(o => o.Body)
                            .ThenBy(o => o.Id)
                        : query
                            .OrderByDescending(o => o.Body)
                            .ThenBy(o => o.Id);
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
