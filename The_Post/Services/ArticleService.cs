using The_Post.Data;
using The_Post.Models;

namespace The_Post.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IHttpContextAccessor _IHttpContextAccessor;
        private readonly ApplicationDbContext _ApplicationDBContext;

        public ArticleService(IHttpContextAccessor iHttpContextAccessor,ApplicationDbContext applicationDbContext)
        {
            _IHttpContextAccessor = iHttpContextAccessor;
            _ApplicationDBContext = applicationDbContext;
        }
        public void CreateArticle(Article article)
        {
            _ApplicationDBContext.Articles.Add(article);
            _ApplicationDBContext.SaveChanges();
        }

        public void DeleteArticle(int articleID)
        {
            var article = _ApplicationDBContext.Articles.FirstOrDefault(a => a.Id == articleID);
            _ApplicationDBContext.Articles.Remove(article);
            _ApplicationDBContext.SaveChanges();
        }

        public void UpdateArticle(Article updatedArticle)
        { 
            _ApplicationDBContext.Articles.Update(updatedArticle);
            _ApplicationDBContext.SaveChanges();

        }
        public List<Article> GetAllArticles() // Pagination
        {
            var article = _ApplicationDBContext.Articles.ToList();
            return article;
        }

        public Article GetArticleById(int articleID)
        {
            var article = _ApplicationDBContext.Articles.FirstOrDefault(c => c.Id == articleID);
            return article;

        }

        public List<Article> GetEditorsChoiceArticles()
        {
            var editorschoice = _ApplicationDBContext.Articles.Where(article => article.EditorsChoice).ToList();
            return editorschoice;
        }

        public List<Article> TenLatestArticles()
        {
            var tenlatest = _ApplicationDBContext.Articles.OrderByDescending(article => article.DateStamp).ToList();
            return tenlatest;
        }

        public List<Article> GetFiveMostPopularArticles()
        {
            var MostPopular = _ApplicationDBContext.Articles.OrderByDescending(m => m.Views).Take(5).ToList();
            return MostPopular;
        }

        public Article GetMostPopularArticleByCategory(int categoryID)
        {
            var mostpopularbycategory = _ApplicationDBContext.Categories.Where(c => c.Id== categoryID)
                                          .SelectMany(c=> c.Articles).OrderByDescending(m => m.Views).FirstOrDefault();
            return (Article) mostpopularbycategory;
        }

        public List<Article> GetAllArticlesByCategory(int categoryID)
        {
            var articles = _ApplicationDBContext.Categories.Where(c => c.Id == categoryID)
                            .SelectMany(c => c.Articles).ToList();
            return articles;

        }

    }
}
