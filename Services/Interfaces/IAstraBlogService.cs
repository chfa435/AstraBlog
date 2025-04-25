using System.Drawing;
using AstraBlog.Models;

namespace AstraBlog.Services.Interfaces
{
    public interface IAstraBlogService
    {
        //BlogPost
        #region BlogPost CRUD Methods
        public Task AddBlogPostAsync(BlogPost blogPost);
        public Task UpdateBlogPostAsync(BlogPost blogPost);
        public Task<BlogPost> GetBlogPostAsync(int blogPostId);
        public Task<BlogPost> GetBlogPostAsync(string blogPostSlug);
        public Task DeleteBlogPostAsync(BlogPost blogPost);
        #endregion

        #region BlogPost Get Methods
        public Task<IEnumerable<BlogPost>> GetBlogPostsAsync();
        public Task<IEnumerable<BlogPost>> GetPopularPostsAsync();
        public Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count);
        public Task<IEnumerable<BlogPost>> GetRecentPostsAsync();
        public Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count);
        public Task<IEnumerable<BlogPost>> GetRecentPostsExceptOneAsync(int blogPostId, int count);
        public Task<IEnumerable<BlogPost>> GetAllPostsAsync();
        #endregion


        //Category
        #region Category CRUD Methods
        public Task AddCategoryAsync(Category category);
        public Task UpdateCategoryAsync(Category category);
        public Task<Category> GetCategoryAsync(int categoryId);
        public Task DeleteCategoryAsync(Category category);
        #endregion

        #region Category Get Method(s)
        public Task<IEnumerable<Category>> GetCategoriesAsync();

        #endregion


        //Tag
        #region Tag CRUD Methods
        public Task AddTagAsync(Tag tag);
        public Task UpdateTagAsync(Tag tag);
        public Task<Tag> GetTagAsync(int tagId);
        public Task DeleteTagAsync(Tag tag);
        #endregion

        #region Tag Get Methods 
        public Task<IEnumerable<Tag>> GetTagsAsync();
        #endregion



        #region Additional Methods
        public Task AddTagsToBlogPostAsync(IEnumerable<int> tagIds, int blogPostId);
        public Task AddTagsToBlogPostAsync(IEnumerable<string> tags, int blogPostId);


        public Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId);

        public Task RemoveAllBlogPostTagsAsync(int blogPostId);

        public IEnumerable<BlogPost> SearchBlogPosts(string? searchString);

        public Task<bool> ValidateSlugAsync(string title, int blogId);
        #endregion
    }
}