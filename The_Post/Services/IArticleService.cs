using System.ComponentModel;

namespace The_Post.Services
{
    public interface IArticleService
    {
        public Task CreateArticle(Article article);
        public Task UpdateArticle(int articleID);
        public Task GetArticleById(int articleID);
        public Task<Article> GetAllArticles(); // Pagination
        public List<Article> GetFiveMostPopularArticles();
        public List<Article> GetEditorsChoiceArticles();
        public Task Article GetMostPopularArticleByCategory(int categoryID); 
        public Task DeleteArticle(int articleID);
    }
}
