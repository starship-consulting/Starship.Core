using System;
using System.Collections.Generic;

namespace Starship.Core.Email {
    public class EmailModel {

        public EmailModel() {
            To = new List<string>();
        }

        public EmailModel(string to, string subject, string body) : this() {
            To.Add(to);
            Subject = subject;
            Body = body;
        }

        public List<string> To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}