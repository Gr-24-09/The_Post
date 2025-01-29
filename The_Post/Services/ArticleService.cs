﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using The_Post.Data;
using The_Post.Models;

namespace The_Post.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IHttpContextAccessor _IHttpContextAccessor;
        private readonly ApplicationDbContext _applicationDBContext;

        public ArticleService(IHttpContextAccessor iHttpContextAccessor,ApplicationDbContext applicationDbContext)
        {
            _IHttpContextAccessor = iHttpContextAccessor;
            _applicationDBContext = applicationDbContext;
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

        public List<Article> GetAllArticles() // Pagination
        {
            var article = _applicationDBContext.Articles.ToList();
            return article;
        }

        public Article GetArticleById(int articleID)
        {
            var article = _applicationDBContext.Articles.FirstOrDefault(c => c.Id == articleID);
            return article;
        }

        public List<Article> GetEditorsChoiceArticles()
        {
            //var editorschoice = _ApplicationDBContext.Articles.
            throw new NotImplementedException();
        }

        public List<Article> GetFiveMostPopularArticles()
        {
            var mostPopular = _applicationDBContext.Articles.OrderByDescending(m => m.Views).Take(5).ToList();
            return mostPopular;
        }

        public Article GetMostPopularArticleByCategory(int categoryID)
        {

            //var articlesInCategory = _applicationDBContext.Categories
            //                .Where(c => c.Id == categoryID)
            //                .SelectMany(c => c.Articles)
            //                .OrderByDescending(a => a.Views).FirstOrDefault();

            throw new NotImplementedException();
        }

        public void UpdateArticle(Article updatedArticle)
        {
            _applicationDBContext.Articles.Update(updatedArticle);
            _applicationDBContext.SaveChanges();
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
    }
}
