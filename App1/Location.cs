
namespace CttApp
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double DistanceTo(Location otherLocation)
        {
            // Implement Haversine formula to calculate distance between two points
            // on the Earth's surface (in meters)
            return 0;
        }
    }
}