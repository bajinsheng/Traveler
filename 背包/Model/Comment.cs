using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 背包.Model
{
    public class Comment
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "idarticle")]
        public string IdArticle { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }
    }
}
