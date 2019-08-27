using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Starship.Core.Email {
    public class EmailClient {

        public EmailClient(string domain, string username, string password, string host, int port = 587) {
            Client = new SmtpClient(host, port);
            Client.EnableSsl = true;
            Client.UseDefaultCredentials = false;
            Client.Credentials = new NetworkCredential(username, password, domain);
        }

        public EmailClient(string username, string password, string host, int port = 587) {
            Client = new SmtpClient(host, port);
            Client.UseDefaultCredentials = false;
            Client.Credentials = new NetworkCredential(username, password);
        }

        public void Send(string from, string to, string subject, string body) {
            Client.Send(GetMailMessage(from, new EmailModel(to, subject, body)));
        }

        public async Task SendAsync(string to, string subject, string body) {
            await Client.SendMailAsync(GetMailMessage(string.Empty, new EmailModel(to, subject, body)));
        }

        public async Task SendAsync(string from, string to, string subject, string body) {
            await Client.SendMailAsync(GetMailMessage(from, new EmailModel(to, subject, body)));
        }

        public async Task SendAsync(string from, EmailModel email) {
            await Client.SendMailAsync(GetMailMessage(from, email));
        }

        private MailMessage GetMailMessage(string from, EmailModel email) {

            if(string.IsNullOrEmpty(from)) {
                from = DefaultFromAddress;
            }

            var message = new MailMessage();

            foreach(var recipient in email.To) {
                message.To.Add(recipient);
            }

            message.From = new MailAddress(from);
            message.Subject = email.Subject;
            
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(email.GetHtml(), Encoding.UTF8, MediaTypeNames.Text.Html));
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(email.GetPlainText(), Encoding.UTF8, MediaTypeNames.Text.Plain));

            return message;
        }

        public string DefaultFromAddress { get; set; }

        private SmtpClient Client { get; set; }
    }
}