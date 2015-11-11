using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace 背包.Model
{
    public class Travel
    {

        public string Id { get; set; }

        [JsonProperty(PropertyName = "idparent")]
        public string IdParent { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "locationorauthor")]
        public string LocationOrAuthor { get; set; }

        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }



    }
}
