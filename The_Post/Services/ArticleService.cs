using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using The_Post.Data;
using The_Post.Models;
using Azure.Storage.Blobs;
using The_Post.Models.VM;

namespace The_Post.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IHttpContextAccessor _IHttpContextAccessor;
        private readonly ApplicationDbContext _applicationDBContext;
        private readonly IConfiguration _configuration;

        public ArticleService(IHttpContextAccessor iHttpContextAccessor,ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _IHttpContextAccessor = iHttpContextAccessor;
            _applicationDBContext = applicationDbContext;
            _configuration = configuration;
        }
        public void CreateArticle(Article article)
        {
            _applicationDBContext.Articles.Add(article);
            _applicationDBContext.SaveChanges();
        }

        public void DeleteArticle(int articleID)
        {
            var article = _applicationDBContext.Articles.FirstOrDefault(a => a.Id == articleID);
            _applicationDBContext.Articles.Remove(article);
            _applicationDBContext.SaveChanges();
        }

        public void UpdateArticle(Article updatedArticle)
        { 
            _applicationDBContext.Articles.Update(updatedArticle);
            _applicationDBContext.SaveChanges();

        }
                
        public async Task<string> UploadFileToContainer(AddArticleVM model)
        {
            string connectionString = _configuration["AzureBlobStorage:ConnectionString"];
            string containerName = _configuration["AzureBlobStorage:ContainerName"];

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create the container if it does not exist

            await containerClient.CreateIfNotExistsAsync();

            string uniqueFileName = $"{Guid.NewGuid()}_{model.ImageLink.FileName}";
            BlobClient blobClient = containerClient.GetBlobClient(uniqueFileName);

            using (var stream = model.ImageLink.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }

        public List<Article> GetAllArticles() // Pagination
        {
            var article = _applicationDBContext.Articles.Include(a => a.Categories).ToList();
            return article;
        }

        public Article GetArticleById(int articleID)
        {
            var article = _applicationDBContext.Articles
                .Include(a => a.Categories)
                .Include(a => a.Likes)
                .FirstOrDefault(c => c.Id == articleID);
            return article;
        }

        public List<Article> GetEditorsChoiceArticles()
        {
            var editorschoice = _applicationDBContext.Articles
                .Include(a => a.Categories)
                .Where(article => article.EditorsChoice).ToList();
            return editorschoice;
        }

        public List<Article> TenLatestArticles()
        {
            var tenlatest = _applicationDBContext.Articles
                .Include(a => a.Categories)
                .OrderByDescending(article => article.DateStamp).Take(10).ToList();
                
            return tenlatest;
        }

        public List<Article> GetFiveMostPopularArticles()
        {
            var mostPopular = _applicationDBContext.Articles
                .Include(a => a.Categories)
                .OrderByDescending(m => m.Views).Take(5).ToList();
            return mostPopular;
        }

        public Article GetMostPopularArticleByCategory(int categoryID)
        {
            var mostpopularbycategory = _applicationDBContext.Categories.Where(c => c.Id == categoryID)
                                          .SelectMany(c => c.Articles)
                                          .Include(a => a.Categories)
                                          .OrderByDescending(m => m.Views).FirstOrDefault();

            return (Article)mostpopularbycategory;
        }

        public List<Article> GetAllArticlesByCategoryID(int categoryID)
        {
            var articles = _applicationDBContext.Categories.Where(c => c.Id == categoryID)
                            .SelectMany(c => c.Articles)
                            .Include(a => a.Categories).ToList();
            return articles;

        }

        public List<Article> GetAllArticlesByCategoryName(string categoryName)
        {
            var articles = _applicationDBContext.Categories.Where(c => c.Name == categoryName)
                            .SelectMany(c => c.Articles)
                            .Include(a => a.Categories).ToList();
            return articles;

        }

        // For use when displaying the categories as checkboxes.
        public List<SelectListItem> GetAllCategoriesSelectList()
        {
            List<SelectListItem> categories = _applicationDBContext.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return categories;
        }

        public List<Category> GetSelectedCategories(List<int> categoryIDs)
        {
            List<Category> categories = _applicationDBContext.Categories
                                        .Where(c => categoryIDs.Contains(c.Id)).ToList();

            return categories;
        }

        public string GetProcessedArticleContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // Splits the text into a list of paragraph-strings and wraps them in <p> tags based on if a double newline is found.
            // Replaces single newlines in a paragraph with a linebreak tag.
            var paragraphs = content.Split(new[] { "\n\n" }, StringSplitOptions.None)
                                  .Select(p => $"<p>{p.Replace("\n", "<br />")}</p>");

            // Joins the paragraphs together into one string.
            return string.Join("", paragraphs);
        }

        // Checks if there is a like made by the user for the article. If there is, the like gets removed. If not, a new like is added.
        public async Task AddRemoveLikeAsync(int articleID, string userID)
        {
            var existingLike = await _applicationDBContext.Likes
                            .FirstOrDefaultAsync(l => l.ArticleId == articleID && l.UserId == userID);

            if (existingLike == null)
            {
                var newLike = new Like
                {
                    ArticleId = articleID,
                    UserId = userID
                };

                await _applicationDBContext.AddAsync(newLike);
            }
            else
                _applicationDBContext.Likes.Remove(existingLike);


            await _applicationDBContext.SaveChangesAsync();
        }

        public async Task<int> GetLikeCountAsync(int articleID)
        {
            var count = await _applicationDBContext.Likes.Where(l => l.ArticleId.Equals(articleID)).CountAsync();

            return count;
        }
    }
}
