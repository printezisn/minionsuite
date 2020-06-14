using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MinionSuiteExample.Web.Models;
using MinionSuiteExample.Web.Services;

namespace MinionSuiteExample.Web.Controllers
{
    public class CommentsController : Controller
    {
        private const int PAGE_SIZE = 20;

        private readonly ICommentService _service;

        public CommentsController(ICommentService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int postId, string term, int page = 1, string sortField = "", bool asc = true)
        {
            var entities = string.IsNullOrWhiteSpace(term)
                ? await _service.GetAllAsync(postId, page, PAGE_SIZE, sortField, asc)
                : await _service.SearchAsync(postId, term, page, PAGE_SIZE, sortField, asc);

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

        public IActionResult Create(int postId)
        {
            var entity = new Comment() { PostId = postId };

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment entity)
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
        public async Task<IActionResult> Edit(Comment entity)
        {
            if (!ModelState.IsValid)
            {
                return View(entity);
            }

            var result = await _service.UpdateAsync(entity);
            if (result == null)
            {
                return NotFound();
            }

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
            var result = await _service.DeleteAsync(id);
            if (result)
            {
                return RedirectToAction("Index", "Posts");
            }

            return NotFound();
        }
    }
}
