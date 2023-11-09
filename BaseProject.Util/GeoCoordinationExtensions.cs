using GeoCoordinatePortable;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace BaseProject.Util
{

	public class Coordinates
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public Coordinates(string latitude, string longitude)
        {
            Latitude = Double.Parse(latitude, new CultureInfo("pt-BR"));
            Longitude = Double.Parse(longitude, new CultureInfo("pt-BR"));
        }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude =longitude;
        }
    }

    public static class GeoCoordinationExtensions
    {

        public static double GetDistanceBetweenLocations(Coordinates origin, Coordinates destiny)
        {

            GeoCoordinate _origin = new GeoCoordinate(origin.Latitude, origin.Longitude);
            GeoCoordinate _destiny = new GeoCoordinate(destiny.Latitude, destiny.Longitude);
            return _origin.GetDistanceTo(_destiny);

        }

        public static Coordinates GetCoordinatesFromApi(string address, string apiKey)
        {
            object parameters = new
            {
                address,
                key = apiKey

            };
            var resultJson = HttpHelper
                .Get<JObject>(
                "https://maps.googleapis.com/maps/api/geocode/json",
                "json",
                parameters);

            var jsonCoordinates = resultJson["results"][0]["geometry"]["location"];
            if (jsonCoordinates != null)
                return new Coordinates(
                    jsonCoordinates["lat"].ToObject<double>(),
                    jsonCoordinates["lng"].ToObject<double>());
            else
                return null;
        }

    }
}
