using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace The_Post.Services
{
    
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]
                ),
                EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"])
            };

            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName), // Uses SenderEmail and SenderName
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };


            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);

            //Email Adress:
            // The.post990@gmail.com

            //App Password :
            //twqy lpqr agjc muww

        }
    }
}



