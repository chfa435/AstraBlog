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
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace AstraBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAstraBlogService _astraBlogService;

        public TagsController(ApplicationDbContext context,
                              IAstraBlogService astraBlogService)
        {
            _context = context;
            _astraBlogService = astraBlogService;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            IEnumerable<Tag> tags = await _astraBlogService.GetTagsAsync();

            return View(tags);
        }

        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }

            Tag tag = await _astraBlogService.GetTagAsync(id.Value);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                await _astraBlogService.AddTagAsync(tag);

                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }

            Tag? tag = await _astraBlogService.GetTagAsync(id.Value);

            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Tag tag)
        {
            if (id != tag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _astraBlogService.UpdateTagAsync(tag);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TagExists(tag.Id))
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
            return View(tag);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }

            Tag? tag = await _astraBlogService.GetTagAsync(id.Value);

            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tags == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tags'  is null.");
            }

            Tag? tag = await _astraBlogService.GetTagAsync(id);

            if (tag != null)
            {
                await _astraBlogService.DeleteTagAsync(tag);    
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TagExists(int id)
        {
            return (await _astraBlogService.GetTagsAsync()).Any(e => e.Id == id);
        }
    }
}
