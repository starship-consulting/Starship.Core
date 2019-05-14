using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Starship.Core.Email {
    public class EmailModel {

        public EmailModel() {
            To = new List<string>();
            CC = new List<string>();
        }

        public EmailModel(string to, string subject, string body) : this() {
            To.Add(to);
            Subject = subject;
            Body = body;
        }

        public string GetPlainText() {

            var body = GetHtml();

            // Remove new lines since they are not visible in HTML
            body = body.Replace("\n", " ");

            // Remove tab spaces
            body = body.Replace("\t", " ");

            // Remove multiple white spaces from HTML
            body = Regex.Replace(body, "\\s+", " ");

            // Remove HEAD tag
            body = Regex.Replace(body, "<head.*?</head>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Remove any JavaScript
            body = Regex.Replace(body, "<script.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Replace special characters like &, <, >, " etc.
            var sbHTML = new StringBuilder(body);

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

        public string GetHtml() {
            return Body.Replace("<p></p>", "").Replace("<p><br></p>", "<br>")
                .Replace("<p", "<div").Replace("</p>", "</div>")
                .Replace("<br><ol>", "<ol>")
                .Replace("<br><ul>", "<ul>")
                .Replace("class=\"ql-align-center\"", "style=\"text-align: center;\"")
                .Replace("class=\"ql-align-right\"", "style=\"text-align: right;\"");
        }

        public List<string> To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<string> CC { get; set; }
    }
}