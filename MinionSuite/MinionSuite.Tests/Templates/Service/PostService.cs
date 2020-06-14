using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinionSuite.Tests.Models;

namespace MinionSuite.Tests.Templates
{
    public class PostService : IPostService
    {
        private readonly ApplicationContext _context;

        public PostService(ApplicationContext context)
        {
            _context = context;
        }

        public virtual async Task<ResultModel<Post>> CreateAsync(Post model)
        {
            var newEntity = new Post();

            newEntity.Title = model.Title;
            newEntity.Body = model.Body;
            newEntity.TotalViews = model.TotalViews;
            newEntity.Rating = model.Rating;
            newEntity.Signature = model.Signature;
            newEntity.CreatedAt = DateTime.UtcNow;
            newEntity.UpdatedAt = DateTime.UtcNow;

            _context.Posts.Add(newEntity);
            await _context.SaveChangesAsync();

            return new ResultModel<Post>(newEntity);
        }

        public virtual async Task<bool> DeleteAsync(int key)
        {
            var entity = await GetAsync(key);
            if (entity == null)
            {
                return false;
            }

            _context.Posts.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public virtual Task<Post> GetAsync(int key)
        {
            return _context.Posts.FirstOrDefaultAsync(f => f.Id == key);
        }

        public virtual Task<List<Post>> GetAllAsync()
        {
            return _context.Posts.AsNoTracking().ToListAsync();
        }

        public virtual Task<PageModel<Post>> GetAllAsync(int page, int pageSize, string sortField, bool asc)
        {
            var query = _context.Posts.AsNoTracking();
            query = GetSortedQuery(query, sortField, asc);

            return PageModel<Post>.CreateAsync(query, page, pageSize, sortField, asc);
        }

        public virtual Task<List<Post>> SearchAsync(string term)
        {
            return _context.Posts.AsNoTracking().Where(w => w.Title.Contains(term) || w.Body.Contains(term)).ToListAsync();
        }

        public virtual Task<PageModel<Post>> SearchAsync(string term, int page, int pageSize, string sortField, bool asc)
        {
            var query = _context.Posts.AsNoTracking().Where(w => w.Title.Contains(term) || w.Body.Contains(term));
            query = GetSortedQuery(query, sortField, asc);

            return PageModel<Post>.CreateAsync(query, page, pageSize, sortField, asc);
        }

        public virtual async Task<ResultModel<Post>> UpdateAsync(Post model)
        {
            var existingEntity = await GetAsync(model.Id);
            if (existingEntity == null)
            {
                return null;
            }

            existingEntity.Title = model.Title;
            existingEntity.Body = model.Body;
            existingEntity.TotalViews = model.TotalViews;
            existingEntity.Rating = model.Rating;
            existingEntity.Signature = model.Signature;
            existingEntity.UpdatedAt = DateTime.UtcNow;

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
                case "TotalViews":
                    return asc
                        ? query
                            .OrderBy(o => o.TotalViews)
                            .ThenBy(o => o.Id)
                        : query
                            .OrderByDescending(o => o.TotalViews)
                            .ThenBy(o => o.Id);
                case "Rating":
                    return asc
                        ? query
                            .OrderBy(o => o.Rating)
                            .ThenBy(o => o.Id)
                        : query
                            .OrderByDescending(o => o.Rating)
                            .ThenBy(o => o.Id);
                case "Signature":
                    return asc
                        ? query
                            .OrderBy(o => o.Signature)
                            .ThenBy(o => o.Id)
                        : query
                            .OrderByDescending(o => o.Signature)
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
