using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using The_Post.Models;

namespace The_Post.Services
{
    public interface IArticleService
    {
        public void CreateArticle(Article article);
        public void UpdateArticle(Article article);
        public Article GetArticleById(int articleID);
        public List<Article> GetAllArticles(); // Pagination
        public List<Article> GetFiveMostPopularArticles();
        public List<Article> GetEditorsChoiceArticles();
        public Article GetMostPopularArticleByCategory(int categoryID);
        public void DeleteArticle(int articleID);
        public List<SelectListItem> GetAllCategoriesSelectList();
        public List<Category> GetSelectedCategories(List<int> categoryIDs);
        public string GetProcessedArticleContent(string content);
    }
}
