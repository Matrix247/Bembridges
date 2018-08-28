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
using Newtonsoft.Json;

namespace Bembridges.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Phone { get; set; }
        [BindProperty]
        public string UserMessage { get; set; }

        private SmtpSettings _smtpSettings;

        public ContactModel(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public void OnGet()
        {
        }

        public ActionResult OnPost(string fullName, string email, string phone, string userMessage)
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string userResponse = Request.Form["g-recaptcha-response"].ToString();
                string url = "https://www.google.com/recaptcha/api/siteverify";
                string myParameters = "secret=6LfOBQoTAAAAAOIbSO7mn7FXLC2gTbj01osP5zqn&response=" + userResponse;
                string jsonResult;

                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    jsonResult = wc.UploadString(url, myParameters);
                }

                var obj = JsonConvert.DeserializeObject<dynamic>(jsonResult);

                if (obj.success == false)
                {
                    Message = "Please click the checkbox to confirm you're not a robot";
                    return Page();
                }

                SendEmail(fullName, email, phone, userMessage);
                Message = "Thank you, your message has been sent";
            }
            catch(Exception)
            {
                Message = "An error has occurred, please try again";
            }

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
            mailMessage.To.Add(_smtpSettings.From);
            mailMessage.Body = body;
            mailMessage.Subject = "New message from customer";
            client.Send(mailMessage);
        }
    }
}
