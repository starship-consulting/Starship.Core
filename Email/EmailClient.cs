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
            Client.Send(GetMailMessage(from, to, subject, body));
        }

        public async Task SendAsync(string from, string to, string subject, string body) {
            await Client.SendMailAsync(GetMailMessage(from, to, subject, body));
        }

        private MailMessage GetMailMessage(string from, string to, string subject, string body) {
            var message = new MailMessage();

            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Subject = subject;

            body = body.Replace("<p></p>", "").Replace("<p><br></p>", "<br>")
                .Replace("<p", "<div").Replace("</p>", "</div>")
                .Replace("<br><ol>", "<ol>")
                .Replace("<br><ul>", "<ul>")
                .Replace("class=\"ql-align-center\"", "style=\"text-align: center;\"")
                .Replace("class=\"ql-align-right\"", "style=\"text-align: right;\"");

            var text = HTMLToText(body);

            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, MediaTypeNames.Text.Html));
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain));

            return message;
        }

        private string HTMLToText(string HTMLCode) {

            // Remove new lines since they are not visible in HTML
            HTMLCode = HTMLCode.Replace("\n", " ");

            // Remove tab spaces
            HTMLCode = HTMLCode.Replace("\t", " ");

            // Remove multiple white spaces from HTML
            HTMLCode = Regex.Replace(HTMLCode, "\\s+", " ");

            // Remove HEAD tag
            HTMLCode = Regex.Replace(HTMLCode, "<head.*?</head>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Remove any JavaScript
            HTMLCode = Regex.Replace(HTMLCode, "<script.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Replace special characters like &, <, >, " etc.
            var sbHTML = new StringBuilder(HTMLCode);

            // Note: There are many more special characters, these are just
            // most common. You can add new characters in this arrays if needed
            string[] OldWords = {
                "&nbsp;", "&amp;", "&quot;", "&lt;",
                "&gt;", "&reg;", "&copy;", "&bull;", "&trade;"
            };

            string[] NewWords = {" ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢"};

            for (int i = 0; i < OldWords.Length; i++) {
                sbHTML.Replace(OldWords[i], NewWords[i]);
            }

            // Check if there are line breaks (<br>) or paragraph (<p>)
            sbHTML.Replace("<br>", "\n<br>");
            sbHTML.Replace("<br ", "\n<br ");
            sbHTML.Replace("<p ", "\n<p ");

            // Finally, remove all HTML tags and return plain text
            return Regex.Replace(sbHTML.ToString(), "<[^>]*>", "");
        }

        private SmtpClient Client { get; set; }
    }
}