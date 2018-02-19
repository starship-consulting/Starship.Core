using System;
using Newtonsoft.Json;

namespace Starship.Core.Security {
    public class SessionToken {

        public SessionToken(Session data, JsonSerializerSettings settings, string secretKey) {
            Data = data;
            SerializerSettings = settings;
            SecretKey = secretKey;
        }

        public void Clear() {
            IssueTime = DateTime.UtcNow.Ticks;
            Signature = String.Empty;
        }

        public void Sign() {
            Clear();

            var data = JsonConvert.SerializeObject(this, SerializerSettings);

            Signature = Hash.HmacSha1(SecretKey, data);
        }

        public String Serialize() {
            return JsonConvert.SerializeObject(this, SerializerSettings);
        }

        public bool Validate() {
            var signature = Signature;

            Signature = String.Empty;

            var token = Hash.HmacSha1(SecretKey, Serialize());

            Signature = signature;

            return signature == token;
        }

        public Session Data { get; set; }

        public long IssueTime { get; set; }

        public string Signature { get; set; }

        private string SecretKey { get; set; }

        private JsonSerializerSettings SerializerSettings { get; set;}
    }
}