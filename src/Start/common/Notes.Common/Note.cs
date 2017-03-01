using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Notes.Common
{
    public class Note
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string EnteredBy { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
    }
}