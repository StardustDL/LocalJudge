using System;
using System.Collections.Generic;
using System.Text;

namespace StarOJ.Server.API.Clients
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.27.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ProblemDetails
    {
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Type { get; set; }

        [Newtonsoft.Json.JsonProperty("title", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Title { get; set; }

        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Status { get; set; }

        [Newtonsoft.Json.JsonProperty("detail", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Detail { get; set; }

        [Newtonsoft.Json.JsonProperty("instance", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Instance { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static ProblemDetails FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemDetails>(data);
        }

    }
}
