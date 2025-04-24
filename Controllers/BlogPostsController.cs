using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AstraBlog.Data;
using AstraBlog.Models;
using Microsoft.AspNetCore.Authorization;
using AstraBlog.Services.Interfaces;
using AstraBlog.Helpers;

namespace AstraBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IAstraBlogService _blogPostService;

        public BlogPostsController(ApplicationDbContext context, IImageService imageService, IAstraBlogService blogPostService)
        {
            _imageService = imageService;
            _blogPostService = blogPostService;
        }

        public async Task<IActionResult> AdminPage()
        {
            var blogPosts = await _blogPostService.GetBlogPostsAsync();
            return View(blogPosts);
        }

        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(slug);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
            //TODO: Add Tags to choose from


            return View(new BlogPost());
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,Status,ImageFile,CategoryId")] BlogPost blogPost, string? stringTags)
        {

            ModelState.Remove("Slug");

            if (ModelState.IsValid)
            {
                // Slug BlogPost
                if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar Title or Slug is already in use.");

                    ViewData["CategoryId"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
                    return View(blogPost);
                }
                blogPost.Slug = StringHelper.BlogSlug(blogPost.Title!);


                // Format Date(s)
                blogPost.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);

                // Image Service
                if (blogPost.ImageFile != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                    blogPost.ImageType = blogPost.ImageFile.ContentType;
                }

                //TODO: Call Service to Save New BlogPost
                await _blogPostService.AddBlogPostAsync(blogPost);


                //TODO: Add Tag(s) to BlogPost
                if (!string.IsNullOrWhiteSpace(stringTags))
                {
                    IEnumerable<string> tags = stringTags.Split(",").ToList();
                    await _blogPostService.AddTagsToBlogPostAsync(tags, blogPost.Id);
                }


                return RedirectToAction("Index","Home");
            }


            ViewData["CategoryId"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);

            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,Status,ImageFile,ImageData,ImageType,CategoryId")] BlogPost blogPost, string? stringTags)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Dates Example
                    blogPost.Created = DataUtility.GetPostGresDate(blogPost.Created);
                    blogPost.Updated = DataUtility.GetPostGresDate(DateTime.UtcNow);

                    // Image Service
                    if (blogPost.ImageFile != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                        blogPost.ImageType = blogPost.ImageFile.ContentType;
                    }

                    // Slug BlogPost
                    if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "A similar Title or Slug is already in use.");

                        ViewData["CategoryId"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
                        return View(blogPost);
                    }
                    blogPost.Slug = StringHelper.BlogSlug(blogPost.Title!);

                    // Call Service to Update BlogPost
                    await _blogPostService.UpdateBlogPostAsync(blogPost);


                    // Edit Tags
                    if (!string.IsNullOrWhiteSpace(stringTags))
                    {
                        IEnumerable<string> tags = stringTags.Split(",").ToList();
                        await _blogPostService.AddTagsToBlogPostAsync(tags, blogPost.Id);
                    }


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AdminPage));
            }
            ViewData["CategoryId"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id);

            if (blogPost != null)
            {
                await _blogPostService.DeleteBlogPostAsync(blogPost);
            }

            return RedirectToAction(nameof(AdminPage));
        }

        private async Task<bool> BlogPostExists(int id)
        {
            return (await _blogPostService.GetBlogPostsAsync()).Any(b => b.Id == id);
        }
    }
}
