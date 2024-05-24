using System;
using Android.Gms.Maps.Model;


namespace CttApp
{
    public class Location
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double Altitude { get; private set; }

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
            double horizontalDistance = SphericalUtil.ComputeDistanceBetween(
                new LatLng(Latitude, Longitude),
                new LatLng(otherLocation.Latitude, otherLocation.Longitude)
            );

            return horizontalDistance;

            /*
            // Calculate the difference in altitude
            double altitudeDifference = otherLocation.Altitude - Altitude;

            // Use the Pythagorean theorem to find the total distance considering altitude
            double totalDistance = Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(altitudeDifference, 2));

            return totalDistance; 
            */
        }
    }
}
