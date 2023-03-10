using AstraBlog.Data;
using AstraBlog.Helpers;
using AstraBlog.Models;
using AstraBlog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AstraBlog.Services
{
    public class BlogPostService : IAstraBlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogPostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                await _context.AddAsync(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddCategoryAsync(Category category)
        {
            try
            {
                await _context.AddAsync(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddTagAsync(Tag tag)
        {
            try
            {
                _context.Add(tag);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task AddTagsToBlogPostAsync(IEnumerable<int> tagIds, int blogPostId)
        {
            throw new NotImplementedException();
        }

        public async Task AddTagsToBlogPostAsync(IEnumerable<string> tags, int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);

                if (blogPost == null)
                {
                    return;
                }

                foreach (string tagName in tags)
                {
                    Tag? tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.Trim().ToLower() == tagName.Trim().ToLower());

                    if (tag != null)
                    {
                        blogPost.Tags.Add(tag);
                    }
                    else
                    {
                        Tag? newTag = new() { Name = tagName };
                        await _context.AddAsync(newTag);

                        blogPost.Tags.Add(newTag);
                    }

                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                blogPost.IsDeleted = true;
                await UpdateBlogPostAsync(blogPost);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            try
            {
                _context.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteTagAsync(Tag tag)
        {
            try
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }        }

        public async Task<BlogPost> GetBlogPostAsync(int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                                                  .Include(b => b.Category)
                                                  .Include(b => b.Tags)
                                                  .Include(b => b.Comments)
                                                  .FirstOrDefaultAsync(b => b.Id == blogPostId);

                return blogPost!;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Get a single BlogPost by Slug (string)
        /// </summary>
        /// <param name="blogPostSlug"></param>
        /// <returns></returns>
        public async Task<BlogPost> GetBlogPostAsync(string blogPostSlug)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                                                  .Include(b => b.Category)
                                                  .Include(b => b.Tags)
                                                  .Include(b => b.Comments)
                                                  .FirstOrDefaultAsync(b => b.Slug == blogPostSlug);

                return blogPost!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetBlogPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                .ToListAsync();

                return blogPosts;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            try
            {
                IEnumerable<Category> categories = await _context.Categories.ToListAsync();

                return categories;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Category> GetCategoryAsync(int categoryId)
        {
            try
            {
                Category? category = await _context.Categories
                                                   .Include(c => c.BlogPosts)
                                                   .FirstOrDefaultAsync(c=> c.Id == categoryId);

                return category!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPopularPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Comments.Count);

            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Gets the specified number of Popular posts based on count.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Comments.Count).Take(count);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Created);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                    .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Created).Take(count);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetRecentPostsExceptOneAsync(int blogPostId, int count)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false && b.Id != blogPostId)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Created).Take(count);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Tag> GetTagAsync(int tagId)
        {
            try
            {
                Tag? tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);

                return tag!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            try
            {
                IEnumerable<Tag> tags = await _context.Tags
                                                      .Include(t => t.BlogPosts)
                                                      .ToListAsync();

                return tags;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllBlogPostTagsAsync(int blogPostId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BlogPost> SearchBlogPosts(string? searchString)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = new List<BlogPost>();

                if (string.IsNullOrEmpty(searchString))
                {
                    return blogPosts;
                }
                else
                {
                    searchString = searchString.Trim().ToLower();

                    blogPosts = _context.BlogPosts.Where(b => b.Title!.ToLower().Contains(searchString) ||
                                                         b.Abstract!.ToLower().Contains(searchString) ||
                                                         b.Content!.ToLower().Contains(searchString) ||
                                                         b.Category!.Name!.ToLower().Contains(searchString) ||
                                                         b.Comments.Any(c => c.Body!.ToLower().Contains(searchString) ||
                                                                        c.Author!.FirstName!.ToLower().Contains(searchString) ||
                                                                        c.Author!.LastName!.ToLower().Contains(searchString)) ||
                                                         b.Tags.Any(t => t.Name!.ToLower().Contains(searchString)))
                                                   .Include(b => b.Comments)
                                                        .ThenInclude(c => c.Author)
                                                   .Include(b => b.Category)
                                                   .Include(b => b.Tags)
                                                   .Where(b => b.IsDeleted == false && b.IsPublished == true)
                                                   .AsNoTracking()
                                                   .OrderByDescending(b => b.Created)
                                                   .AsEnumerable();

                    return blogPosts;   
                }

            }
            catch (Exception)
            {

                throw;
            }        
        }

        public async Task UpdateBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                _context.Update(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            try
            {
                _context.Update(tag);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }        
        }

        public async Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            try
            {
                string newSlug = StringHelper.BlogSlug(title);

                if (blogId == 0)
                {
                    return !await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug);
                }
                else
                {
                    BlogPost? blogPost = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == blogId);

                    string? oldSlug = blogPost?.Slug;

                    if (!string.Equals(oldSlug, newSlug))
                    {
                        return !await _context.BlogPosts.AnyAsync(b => b.Id != blogPost!.Id && b.Slug == newSlug);
                    }
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
