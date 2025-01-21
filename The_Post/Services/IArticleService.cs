using System.ComponentModel;
using The_Post.Models;

namespace The_Post.Services
{
    public interface IArticleService
    {
        public Task CreateArticle(Article article);
        public Task UpdateArticle(int articleID);
        public Task GetArticleById(int articleID);
        public Task<Article> GetAllArticles(); // Pagination
        public Task<Article> GetFiveMostPopularArticles();
        public Task<Article> GetEditorsChoiceArticles();
        public Task GetMostPopularArticleByCategory(int categoryID); 
        public Task DeleteArticle(int articleID);
    }
}
