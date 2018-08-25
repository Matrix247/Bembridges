using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Bembridges.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Bembridges.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }
        private SmtpSettings _smtpSettings;

        public ContactModel(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public void OnGet()
        {
        }

        public ActionResult OnPost(string name, string email, string phone, string message)
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            SendEmail(name, email, phone, message);
            Message = "Thank you, your message has been sent";

            return null;
        }

        private void SendEmail(string name, string email, string phone, string message)
        {
            SmtpClient client = new SmtpClient(_smtpSettings.Server)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password)
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.From)
            };

            string body = "<html><head><style>p{padding: 5px;}" + $"</style></head><body><p>New message from {name}</p><p>Email: {email}<br/>Phone: {phone}</p><p>Message:<br/>{message}</p></body></html>";

            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add("steven.colls@btinternet.com");
            mailMessage.Body = body;
            mailMessage.Subject = "New message from customer";
            client.Send(mailMessage);
        }
    }
}
