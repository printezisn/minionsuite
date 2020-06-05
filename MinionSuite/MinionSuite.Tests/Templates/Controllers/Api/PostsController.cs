using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MinionSuite.Tests.Models;

namespace MinionSuite.Tests.Templates.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private const int PAGE_SIZE = 20;

        private readonly IPostService _service;

        public PostsController(IPostService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string term, int page = 1, string sortField = "", bool asc = true)
        {
            var entities = string.IsNullOrWhiteSpace(term)
                ? await _service.GetAllAsync(page, PAGE_SIZE, sortField, asc)
                : await _service.SearchAsync(term, page, PAGE_SIZE, sortField, asc);

            return Ok(entities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _service.GetAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post entity)
        {
            var result = await _service.CreateAsync(entity);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Post entity)
        {
            var result = await _service.UpdateAsync(entity);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
