using System;

namespace Starship.Core.Security {
    public class Session : ISession {

        public Session() {
        }

        public Session(ISession session) {
            Username = session.Username;
        }

        public string AccountId { get; set; }

        public string Username { get; set; }
    }
}