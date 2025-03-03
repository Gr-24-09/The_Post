using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using The_Post.Data;
using The_Post.Models;
using static System.Net.WebRequestMethods;

namespace SendNewsletters_Isolated
{
    public class SendNewsletter
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;

        public SendNewsletter(ILoggerFactory loggerFactory, ApplicationDbContext dbContext, IEmailSender emailSender)
        {
            _logger = loggerFactory.CreateLogger<SendNewsletter>();
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }


            // Get all users who have opted in to the newsletter
            var users = _dbContext.Users
                .Include(u => u.NewsletterCategories)
                .Where(u => u.EditorsChoiceNewsletter == true || u.NewsletterCategories.Any()).ToList();

            var allArticles = _dbContext.Articles
                .Include(a => a.Categories)
                .Where(a => a.IsArchived == false).ToList();

            var fetchedEditorsChoiceArticles = allArticles.Where(a => a.EditorsChoice).ToList();

            foreach (var user in users)
            {
                List<List<Article>> articlesByCategory = new List<List<Article>>();
                List<Article> articlesEditorsChoice = new List<Article>();

                if(user.EditorsChoiceNewsletter)
                {
                    articlesEditorsChoice = fetchedEditorsChoiceArticles;
                }

                if (user.NewsletterCategories.Count != 0)
                {
                    foreach (var category in user.NewsletterCategories)
                    {
                        var articles = allArticles
                            .Where(a => a.Categories.Contains(category))
                            .OrderByDescending(a => a.DateStamp)
                            .Take(5).ToList();
                        articlesByCategory.Add(articles);
                    }
                }

                List<string> categoryNames = user.NewsletterCategories.Select(c => c.Name).ToList();

                var emailContent =  BuildEmailHtml(articlesByCategory, categoryNames, fetchedEditorsChoiceArticles);


                // Send the newsletter
                try
                {
                    await _emailSender.SendEmailAsync(user.Email, "Your Weekly Newsletter", emailContent);
                    _logger.LogInformation($"Sending newsletter to {user.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send newsletter to {user.Email}: {ex.Message}");
                }
            }

        }


        public string BuildEmailHtml(List<List<Article>> articlesByCategory, List<string> categoryNames, List<Article> articlesEditorsChoice)
        {
            var htmlContent = "<html><head>";

            // Email-safe styles
            htmlContent += @"
            <style>
                body {
                    text-align: center;
                    font-family: Arial, sans-serif;
                }
                .responsive-content {
                    width: 100%;
                    max-width: 600px; /* Set a max width */
                    margin: 0 auto;
                }
                img {
                    width: 100%;
                    max-width: 600px; /* Set a max width */
                    height: auto;
                    margin: 0 auto;
                    display: block;
                }
            </style>";

            htmlContent += "</head><body style='text-align:center'>";
            htmlContent += "<h1>Your Weekly Newsletter</h1>";

            // Wrap all content in a responsive container
            htmlContent += "<div class='responsive-content'>";

            // Insert the Editors Choice section
            htmlContent += BuildEditorsChoiceHtml(articlesEditorsChoice);

            // Insert each category section
            int categoryIndex = 0;
            foreach (var categoryArticles in articlesByCategory)
            {
                htmlContent += BuildCategoryHtml(categoryArticles, categoryNames[categoryIndex++]);
            }

            // Close the responsive container and the HTML document
            htmlContent += "</div></body></html>";

            return htmlContent;
        }

        string baseUrlViewArticle = "https://localhost:7116/Article/ViewArticle?articleID=";


        public string BuildEditorsChoiceHtml(List<Article> articles)
        {
            var htmlContent = "<h2>Editors Choice</h2><ul style='display: inline-block; padding: 0;'>";

            foreach (var article in articles)
            {
                var articleUrl = baseUrlViewArticle + article.Id;
                htmlContent += $@"
                <li style='list-style: none; text-align: center; margin-bottom: 40px;'>
                    <div>
                        <img src='{article.ImageOriginalLink}'/>
                    </div>
                    <h3>
                        <a href='{articleUrl}'>{article.HeadLine}</a>
                    </h3>
                    <p style='text-align: left;'>{article.ContentSummary}</p>
                    <hr style='margin-top: 25px'>
                </li>";
            }
            htmlContent += "</ul>";
            return htmlContent;
        }

        public string BuildCategoryHtml(List<Article> articles, string category)
        {
            var htmlContent = $"<h2>The latest in {category} news</h2><ul style='display: inline-block; padding: 0;'>";

            foreach (var article in articles)
            {
                var articleUrl = baseUrlViewArticle + article.Id;
                htmlContent += $@"
                <li style='list-style: none; text-align: center; margin-bottom: 40px;'>
                    <div>
                        <img src='{article.ImageOriginalLink}' />
                    </div>
                    <h3>
                        <a href='{articleUrl}'>{article.HeadLine}</a>
                    </h3>
                    <p style='text-align: left;'>{article.ContentSummary}</p>
                    <hr style='margin-top: 25px'>
                </li>";
            }
            htmlContent += "</ul>";
            return htmlContent;
        }



    }
}
