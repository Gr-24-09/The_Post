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

        //public List<Article> GetEditorsChoiceArticles()
        //{
        //    var editorschoice = _ApplicationDBContext.Articles.
        //}

        public List<Article> GetFiveMostPopularArticles()
        {
            var MostPopular = _ApplicationDBContext.Articles.OrderByDescending(m => m.Views).Take(5).ToList();
            return MostPopular;
        }

        public Article GetMostPopularArticleByCategory(int categoryID)
        {
            var mostpopularbycategory = _ApplicationDBContext.Articles.GroupBy(c => c.Categories).
                                          Select(group => group.OrderByDescending(m => m.Views).FirstOrDefault());
            return (Article) mostpopularbycategory;
        }

        public void UpdateArticle(int articleID,Article updatedarticle)
        {
            var existingarticle = _ApplicationDBContext.Articles.FirstOrDefault(a => a.Id == articleID);
                if(existingarticle !=null )
            {
                existingarticle.DateStamp = updatedarticle.DateStamp;
                existingarticle.HeadLine = updatedarticle.HeadLine;
                existingarticle.LinkText = updatedarticle.LinkText;
                existingarticle.ContentSummary = updatedarticle.ContentSummary;
                existingarticle.Content = updatedarticle.Content;
                existingarticle.Views = updatedarticle.Views;
                existingarticle.Likes = updatedarticle.Likes;
                existingarticle.ImageLink = updatedarticle.ImageLink;
                existingarticle.IsArchived = updatedarticle.IsArchived;

                _ApplicationDBContext.SaveChanges();
            }
        }
    }
}
