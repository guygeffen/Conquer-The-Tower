using System;
using Android.Gms.Maps.Model;


namespace CttApp
{
    public class Location
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
       

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            
        }


        public double DistanceTo(Location otherLocation)
        {
            double horizontalDistance = SphericalUtil.ComputeDistanceBetween(
                new LatLng(Latitude, Longitude),
                new LatLng(otherLocation.Latitude, otherLocation.Longitude)
            );

            return horizontalDistance;

         
        }

        public LatLng GetLatLng()
        {
            return new LatLng(this.Latitude, this.Longitude);
        }
    }
}
