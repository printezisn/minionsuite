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
        private ApplicationContext _context;
        private PostService _service;

        public PostServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase("PostService")
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

            entity.Title = "2";
            await _service.UpdateAsync(entity);

            entity = await _service.GetAsync(entity.Id);

            Assert.NotNull(entity);
            Assert.Equal("2", entity.Title);
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
            entity.Title = "2";
            await _service.UpdateAsync(entity);

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
            var secondEntity = await CreateEntity();
            secondEntity.Title = "2";
            secondEntity.Body = "2";
            await _service.UpdateAsync(secondEntity);

            var entities = await _service.SearchAsync("2");

            Assert.Single(entities);
            Assert.Equal(secondEntity.Id, entities.First().Id);
        }

        [Fact]
        public async Task TestSearchWithPagingAndSorting()
        {
            await CreateEntity();
            var secondEntity = await CreateEntity();
            var thirdEntity = await CreateEntity();
            thirdEntity.Title = "2";
            thirdEntity.Body = "2";
            await _service.UpdateAsync(thirdEntity);

            var page = await _service.SearchAsync("1", 1, 1, "Id", false);

            Assert.Equal(2, page.TotalItems);
            Assert.Equal(1, page.Page);
            Assert.Equal(1, page.PageSize);
            Assert.Equal("Id", page.SortField);
            Assert.False(page.IsAscending);
            Assert.Equal(secondEntity.Id, page.First().Id);
        }

        private async Task<Post> CreateEntity()
        {
            var entity = new Post()
            {
                Title = "1",
                Body = "1",
                TotalViews = 1,
                Rating = 1,
            };

            var result = await _service.CreateAsync(entity);
            return result.Result;
        }
    }
}
