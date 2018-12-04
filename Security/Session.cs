using System;

namespace Starship.Core.Security {
    public class Session : ISession {

        public Session() {
        }

        public Session(ISession session) {
            Email = session.Email;
        }

        public string GetPassword() {
            return Password;
        }
        
        public bool IsAuthenticated { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        private string Password { get; set; }
    }
}