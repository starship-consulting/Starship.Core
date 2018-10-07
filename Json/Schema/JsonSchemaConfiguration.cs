namespace Starship.Core.Json.Schema {
    public class JsonSchemaConfiguration {
        public static JsonSchemaConfiguration Default {
            get { return new JsonSchemaConfiguration(); }
        }

        public bool SimpleOutput { get; set; }
    }
}