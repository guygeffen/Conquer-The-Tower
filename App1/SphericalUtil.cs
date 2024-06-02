/*
 * Copyright 2013 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * This class was converted from Java to C# using chat GPT, remmoving unused functions
 */

using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;
using static System.Math;

namespace CttApp
{
    public static class SphericalUtil
    {
        private const double EarthRadius = 6371000; // in meters

  

        public static double ComputeHeading(LatLng from, LatLng to)
        {
            double fromLat = ToRadians(from.Latitude);
            double fromLng = ToRadians(from.Longitude);
            double toLat = ToRadians(to.Latitude);
            double toLng = ToRadians(to.Longitude);
            double dLng = toLng - fromLng;
            double heading = Atan2(
                Sin(dLng) * Cos(toLat),
                Cos(fromLat) * Sin(toLat) - Sin(fromLat) * Cos(toLat) * Cos(dLng));
            return Wrap(ToDegrees(heading), -180, 180);
        }

        public static LatLng ComputeOffset(LatLng from, double distance, double heading)
        {
            distance /= EarthRadius;
            heading = ToRadians(heading);
            double fromLat = ToRadians(from.Latitude);
            double fromLng = ToRadians(from.Longitude);
            double cosDistance = Cos(distance);
            double sinDistance = Sin(distance);
            double sinFromLat = Sin(fromLat);
            double cosFromLat = Cos(fromLat);
            double sinLat = cosDistance * sinFromLat + sinDistance * cosFromLat * Cos(heading);
            double dLng = Atan2(
                sinDistance * cosFromLat * Sin(heading),
                cosDistance - sinFromLat * sinLat);
            return new LatLng(ToDegrees(Asin(sinLat)), ToDegrees(fromLng + dLng));
        }

       
       

        private static double DistanceRadians(double lat1, double lng1, double lat2, double lng2)
        {
            return ArcHav(HavDistance(lat1, lat2, lng1 - lng2));
        }

        static double ComputeAngleBetween(LatLng from, LatLng to)
        {
            return DistanceRadians(ToRadians(from.Latitude), ToRadians(from.Longitude),
                ToRadians(to.Latitude), ToRadians(to.Longitude));
        }

        public static double ComputeDistanceBetween(LatLng from, LatLng to)
        {
            return ComputeAngleBetween(from, to) * EarthRadius;
        }

       

       

        private static double ToRadians(double degrees)
        {
            return degrees * PI / 180;
        }

        private static double ToDegrees(double radians)
        {
            return radians * 180 / PI;
        }

        private static double Wrap(double n, double min, double max)
        {
            return n < min ? n + (max - min) : (n >= max ? n - (max - min) : n);
        }

        private static double ArcHav(double x)
        {
            return 2 * Asin(Sqrt(x));
        }

        private static double Hav(double x)
        {
            double sinHalf = Sin(x * 0.5);
            return sinHalf * sinHalf;
        }

        private static double HavDistance(double lat1, double lat2, double dLng)
        {
            return Hav(lat1 - lat2) + Cos(lat1) * Cos(lat2) * Hav(dLng);
        }
    }

 
}
