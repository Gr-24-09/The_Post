using System.ComponentModel;
using The_Post.Models;

namespace The_Post.Services
{
    public interface IArticleService
    {
        public void CreateArticle(Article article);
        public void UpdateArticle(Article updateArticle);
        public Article GetArticleById(int articleID);
        public List<Article> GetAllArticles(); // Pagination
        public List<Article> GetFiveMostPopularArticles();
        public List<Article> GetEditorsChoiceArticles();

        public List<Article> TenLatestArticles();
        public Article GetMostPopularArticleByCategory(int categoryID);
        public void DeleteArticle(int articleID);
    }
}
