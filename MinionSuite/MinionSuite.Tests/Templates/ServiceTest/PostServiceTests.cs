using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinionSuite.Tests.Models;
using Xunit;

namespace MinionSuite.Tests.Templates
{
    public class PostServiceTests : IDisposable
    {
        private int _entitySequence = 1;

        private ApplicationContext _context;
        private PostService _service;

        public PostServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase("Application")
                .Options;
            _context = new ApplicationContext(options);
            _service = new PostService(_context);
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
            var entities = await _service.GetAllAsync();

            Assert.Single(entities);
            Assert.Equal(entity.Title, entities[0].Title);
            Assert.Equal(entity.Body, entities[0].Body);
            Assert.Equal(entity.TotalViews, entities[0].TotalViews);
            Assert.Equal(entity.Rating, entities[0].Rating);
        }

        [Fact]
        public async Task TestUpdateEntity()
        {
            var entity = await CreateEntity();

            entity.Title = _entitySequence.ToString();
            await _service.UpdateAsync(entity);

            entity = await _service.GetAsync(entity.Id);

            Assert.NotNull(entity);
            Assert.Equal(_entitySequence.ToString(), entity.Title);
        }

        [Fact]
        public async Task TestUpdateNotExistingEntity()
        {
            var entity = new Post()
            {
                Id = _entitySequence
            };

            var result = await _service.UpdateAsync(entity);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task TestDeleteEntity()
        {
            var entity = await CreateEntity();
            await _service.DeleteAsync(entity);

            entity = await _service.GetAsync(entity.Id);

            Assert.Null(entity);
        }

        [Fact]
        public async Task TestPagingAndSorting()
        {
            await CreateEntity();
            var entity = await CreateEntity();

            var page = await _service.GetAllAsync(1, 1, "Title", false);

            Assert.Equal(2, page.TotalItems);
            Assert.Equal(1, page.Page);
            Assert.Equal(1, page.PageSize);
            Assert.Equal("Title", page.SortField);
            Assert.False(page.IsAscending);
            Assert.Equal(entity.Id, page.First().Id);
        }

        [Fact]
        public async Task TestSearch()
        {
            await CreateEntity();
            var entity = await CreateEntity();

            var entities = await _service.SearchAsync(entity.Title);

            Assert.Single(entities);
            Assert.Equal(entity.Id, entities.First().Id);
        }

        [Fact]
        public async Task TestSearchWithPagingAndSorting()
        {
            await CreateEntity();
            var entity = await CreateEntity();

            var page = await _service.SearchAsync(entity.Title, 1, 1, "Id", false);

            Assert.Equal(1, page.TotalItems);
            Assert.Equal(1, page.Page);
            Assert.Equal(1, page.PageSize);
            Assert.Equal("Id", page.SortField);
            Assert.False(page.IsAscending);
            Assert.Equal(entity.Id, page.First().Id);
        }

        private async Task<Post> CreateEntity()
        {
            var entity = new Post()
            {
                Title = _entitySequence.ToString(),
                Body = _entitySequence.ToString(),
                TotalViews = _entitySequence,
                Rating = _entitySequence,
            };

            var result = await _service.CreateAsync(entity);

            _entitySequence++;

            return result.Result;
        }
    }
}
