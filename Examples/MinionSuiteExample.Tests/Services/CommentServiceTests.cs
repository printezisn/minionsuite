using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinionSuiteExample.Web.Data;
using MinionSuiteExample.Web.Models;
using MinionSuiteExample.Web.Services;
using Xunit;

namespace MinionSuiteExample.Tests.Services
{
    public class CommentServiceTests : IDisposable
    {
        private int _entitySequence = 1;

        private ApplicationDbContext _context;
        private CommentService _service;

        public CommentServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("CommentService")
                .Options;
            _context = new ApplicationDbContext(options);
            _service = new CommentService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task TestCreateAndReturnEntities()
        {
            var entity = await CreateEntity();
            var entities = await _service.GetAllAsync(entity.PostId);

            Assert.Single(entities);
            Assert.Equal(entity.Body, entities[0].Body);
            Assert.Equal(entity.PostId, entities[0].PostId);
        }

        [Fact]
        public async Task TestCreateWithNonExistingPost()
        {
            var entity = new Comment()
            {
                Body = _entitySequence.ToString(),
            };

            var result = await _service.CreateAsync(entity);

            Assert.False(result.IsSuccess);
            Assert.Contains("The post was not found.", result.Errors);
        }

        [Fact]
        public async Task TestUpdateEntity()
        {
            var entity = await CreateEntity();

            entity.Body = _entitySequence.ToString();
            await _service.UpdateAsync(entity);

            entity = await _service.GetAsync(entity.Id);

            Assert.NotNull(entity);
            Assert.Equal(_entitySequence.ToString(), entity.Body);
        }

        [Fact]
        public async Task TestUpdateNotExistingEntity()
        {
            var entity = new Comment()
            {
                Id = _entitySequence
            };

            var result = await _service.UpdateAsync(entity);

            Assert.Null(result);
        }

        [Fact]
        public async Task TestDeleteEntity()
        {
            var entity = await CreateEntity();
            await _service.DeleteAsync(entity.Id);

            entity = await _service.GetAsync(entity.Id);

            Assert.Null(entity);
        }

        [Fact]
        public async Task TestPagingAndSorting()
        {
            await CreateEntity();
            var firstEntity = await CreateEntity();
            var secondEntity = await CreateEntity(firstEntity.PostId);

            var page = await _service.GetAllAsync(firstEntity.PostId, 1, 1, "CreatedAt", false);

            Assert.Equal(2, page.TotalItems);
            Assert.Equal(1, page.Page);
            Assert.Equal(1, page.PageSize);
            Assert.Equal("CreatedAt", page.SortField);
            Assert.False(page.IsAscending);
            Assert.Equal(secondEntity.Id, page.First().Id);
        }

        [Fact]
        public async Task TestSearch()
        {
            var entity = await CreateEntity();
            await CreateEntity(entity.PostId);

            var entities = await _service.SearchAsync(entity.PostId, entity.Body);

            Assert.Single(entities);
            Assert.Equal(entity.Id, entities.First().Id);
        }

        [Fact]
        public async Task TestSearchWithPagingAndSorting()
        {
            var entity = await CreateEntity();
            await CreateEntity(entity.PostId);

            var page = await _service.SearchAsync(entity.PostId, entity.Body, 1, 1, "Id", false);

            Assert.Equal(1, page.TotalItems);
            Assert.Equal(1, page.Page);
            Assert.Equal(1, page.PageSize);
            Assert.Equal("Id", page.SortField);
            Assert.False(page.IsAscending);
            Assert.Equal(entity.Id, page.First().Id);
        }

        private async Task<Comment> CreateEntity(int? postId = null)
        {
            if (!postId.HasValue)
            {
                var post = new Post();
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                postId = post.Id;
            }

            var entity = new Comment()
            {
                Body = _entitySequence.ToString(),
                PostId = postId.Value,
            };

            var result = await _service.CreateAsync(entity);

            _entitySequence++;

            return result.Result;
        }
    }
}
