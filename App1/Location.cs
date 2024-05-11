
using System;

namespace CttApp
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Altitude { get; set; }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = 0;
        }

        public Location(double latitude, double longitude, double altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public double DistanceTo(Location otherLocation)
        {
            double earthRadius = 6371000; // Earth's radius in meters

            double lat1Radians = Latitude * (Math.PI / 180);
            double lat2Radians = otherLocation.Latitude * (Math.PI / 180);
            double deltaLatRadians = (otherLocation.Latitude - Latitude) * (Math.PI / 180);
            double deltaLonRadians = (otherLocation.Longitude - Longitude) * (Math.PI / 180);

            // Haversine formula to calculate the great-circle distance between two points
            double a = Math.Sin(deltaLatRadians / 2) * Math.Sin(deltaLatRadians / 2) +
                       Math.Cos(lat1Radians) * Math.Cos(lat2Radians) *
                       Math.Sin(deltaLonRadians / 2) * Math.Sin(deltaLonRadians / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double horizontalDistance = earthRadius * c; // Horizontal distance on the Earth's surface

            // Calculate the difference in altitude
            double altitudeDifference = otherLocation.Altitude - Altitude;

            // Use the Pythagorean theorem to find the total distance considering altitude
            double totalDistance = Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(altitudeDifference, 2));

            return totalDistance;
        }
    }
}