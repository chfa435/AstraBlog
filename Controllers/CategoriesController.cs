using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AstraBlog.Data;
using AstraBlog.Models;
using AstraBlog.Services.Interfaces;
using AstraBlog.Services;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace AstraBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly IImageService _imageService; 
        private readonly IAstraBlogService _blogPostService;

        public CategoriesController(IAstraBlogService blogPostService, 
                                    IImageService imageService)
        {
            _blogPostService = blogPostService;
            _imageService = imageService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _blogPostService.GetCategoriesAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id, int? pageNum = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _blogPostService.GetCategoryAsync(id.Value);

            if (category == null)
            {
                return NotFound();
            }

            ViewData["PageNum"] = pageNum;  

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageFile")] Category category)
        {
            if (ModelState.IsValid)
            {

                // Image Service
                if (category.ImageFile != null)
                {
                    category.ImageData = await _imageService.ConvertFileToByteArrayAsync(category.ImageFile);
                    category.ImageType = category.ImageFile.ContentType;
                }

                await _blogPostService.AddCategoryAsync(category);   
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _blogPostService.GetCategoryAsync(id.Value);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageData,ImageType")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    // Image Service
                    if (category.ImageFile != null)
                    {
                        category.ImageData = await _imageService.ConvertFileToByteArrayAsync(category.ImageFile);
                        category.ImageType = category.ImageFile.ContentType;
                    }

                    await _blogPostService.UpdateCategoryAsync(category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _blogPostService.GetCategoryAsync(id.Value);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var category = await _blogPostService.GetCategoryAsync(id);

            if (category != null)
            {
                await _blogPostService.DeleteCategoryAsync(category);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CategoryExists(int id)
        {
          return (await _blogPostService.GetCategoriesAsync()).Any(c => c.Id == id);
        }
    }
}
