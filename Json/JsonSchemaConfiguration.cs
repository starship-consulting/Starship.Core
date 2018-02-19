namespace Starship.Core.Json {
    public class JsonSchemaConfiguration {
        public static JsonSchemaConfiguration Default {
            get { return new JsonSchemaConfiguration(); }
        }

        public bool SimpleOutput { get; set; }
    }
}