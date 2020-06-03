using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MinionSuite.Tests.Models;

namespace MinionSuite.Tests.Templates
{
    public class PostsController : Controller
    {
        private const int PAGE_SIZE = 20;

        private readonly IPostService _service;

        public PostsController(IPostService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string term, int page = 1, string sortField = "", bool asc = true)
        {
            var entities = string.IsNullOrWhiteSpace(term)
                ? await _service.GetAllAsync(page, PAGE_SIZE, sortField, asc)
                : await _service.SearchAsync(term, page, PAGE_SIZE, sortField, asc);

            return View(entities);
        }

        public async Task<IActionResult> Details(int id)
        {
            var entity = await _service.GetAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Body,TotalViews,Rating")] Post entity)
        {
            if (!ModelState.IsValid)
            {
                return View(entity);
            }

            var result = await _service.CreateAsync(entity);
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(entity);
            }

            return RedirectToAction(nameof(Details), new { id = result.Result.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _service.GetAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Title,Body,TotalViews,Rating")] Post entity)
        {
            if (!ModelState.IsValid)
            {
                return View(entity);
            }

            var result = await _service.UpdateAsync(entity);
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(entity);
            }

            return RedirectToAction(nameof(Details), new { id = result.Result.Id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _service.GetAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _service.GetAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(entity);

            return RedirectToAction(nameof(Index));
        }
    }
}
