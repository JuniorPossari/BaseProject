using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseProject.DAO.Models.Others
{
	public class Estado
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Nome { get; set; }

        [JsonProperty("iso2")]
        public string Codigo { get; set; }

        [JsonProperty("country_id")]
        public int IdPais { get; set; }

        [JsonProperty("country_code")]
        public string CodigoPais { get; set; }

        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }
    }
}
