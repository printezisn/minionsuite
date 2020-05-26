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
            var query = GetSortedQuery(sortField, asc);

            return PageModel<Post>.CreateAsync(query, page, pageSize);
        }

        public virtual Task<List<Post>> SearchAsync(string term)
        {
            return _context.Posts.Where(w => w.Title.Contains(term) || w.Body.Contains(term)).ToListAsync();
        }

        public virtual Task<PageModel<Post>> SearchAsync(string term, int page, int pageSize, string sortField, bool asc)
        {
            var query = GetSortedQuery(sortField, asc).Where(w => w.Title.Contains(term) || w.Body.Contains(term));

            return PageModel<Post>.CreateAsync(query, page, pageSize);
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

        protected virtual IQueryable<Post> GetSortedQuery(string sortField, bool asc)
        {
            var query = _context.Posts.OrderBy(o => o.Id);

            switch (sortField)
            {
                case "Title":
                    query = asc
                        ? _context.Posts
                            .OrderBy(o => o.Title)
                            .ThenBy(o => o.Id)
                        : _context.Posts
                            .OrderByDescending(o => o.Title)
                            .ThenBy(o => o.Id);
                    break;
                case "Body":
                    query = asc
                        ? _context.Posts
                            .OrderBy(o => o.Body)
                            .ThenBy(o => o.Id)
                        : _context.Posts
                            .OrderByDescending(o => o.Body)
                            .ThenBy(o => o.Id);
                    break;
                case "CreatedAt":
                    query = asc
                        ? _context.Posts
                            .OrderBy(o => o.CreatedAt)
                            .ThenBy(o => o.Id)
                        : _context.Posts
                            .OrderByDescending(o => o.CreatedAt)
                            .ThenBy(o => o.Id);
                    break;
                case "UpdatedAt":
                    query = asc
                        ? _context.Posts
                            .OrderBy(o => o.UpdatedAt)
                            .ThenBy(o => o.Id)
                        : _context.Posts
                            .OrderByDescending(o => o.UpdatedAt)
                            .ThenBy(o => o.Id);
                    break;
            }

            return query;
        }
    }
}
