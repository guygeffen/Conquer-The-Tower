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
 * This class was converted from Jave to C# using chat GPT.
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

        public static LatLng ComputeOffsetOrigin(LatLng to, double distance, double heading)
        {
            heading = ToRadians(heading);
            distance /= EarthRadius;
            double n1 = Cos(distance);
            double n2 = Sin(distance) * Cos(heading);
            double n3 = Sin(distance) * Sin(heading);
            double n4 = Sin(ToRadians(to.Latitude));
            double n12 = n1 * n1;
            double discriminant = n2 * n2 * n12 + n12 * n12 - n12 * n4 * n4;
            if (discriminant < 0)
            {
                return null;
            }
            double b = n2 * n4 + Sqrt(discriminant);
            b /= n1 * n1 + n2 * n2;
            double a = (n4 - n2 * b) / n1;
            double fromLatRadians = Atan2(a, b);
            if (fromLatRadians < -PI / 2 || fromLatRadians > PI / 2)
            {
                b = n2 * n4 - Sqrt(discriminant);
                b /= n1 * n1 + n2 * n2;
                fromLatRadians = Atan2(a, b);
            }
            if (fromLatRadians < -PI / 2 || fromLatRadians > PI / 2)
            {
                return null;
            }
            double fromLngRadians = ToRadians(to.Longitude) -
                Atan2(n3, n1 * Cos(fromLatRadians) - n2 * Sin(fromLatRadians));
            return new LatLng(ToDegrees(fromLatRadians), ToDegrees(fromLngRadians));
        }

        public static LatLng Interpolate(LatLng from, LatLng to, double fraction)
        {
            double fromLat = ToRadians(from.Latitude);
            double fromLng = ToRadians(from.Longitude);
            double toLat = ToRadians(to.Latitude);
            double toLng = ToRadians(to.Longitude);
            double cosFromLat = Cos(fromLat);
            double cosToLat = Cos(toLat);

            double angle = ComputeAngleBetween(from, to);
            double sinAngle = Sin(angle);
            if (sinAngle < 1E-6)
            {
                return new LatLng(
                    from.Latitude + fraction * (to.Latitude - from.Latitude),
                    from.Longitude + fraction * (to.Longitude - from.Longitude));
            }
            double a = Sin((1 - fraction) * angle) / sinAngle;
            double b = Sin(fraction * angle) / sinAngle;

            double x = a * cosFromLat * Cos(fromLng) + b * cosToLat * Cos(toLng);
            double y = a * cosFromLat * Sin(fromLng) + b * cosToLat * Sin(toLng);
            double z = a * Sin(fromLat) + b * Sin(toLat);

            double lat = Atan2(z, Sqrt(x * x + y * y));
            double lng = Atan2(y, x);
            return new LatLng(ToDegrees(lat), ToDegrees(lng));
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

        public static double ComputeLength(List<LatLng> path)
        {
            if (path.Count < 2)
            {
                return 0;
            }
            double length = 0;
            LatLng prev = null;
            foreach (LatLng point in path)
            {
                if (prev != null)
                {
                    double prevLat = ToRadians(prev.Latitude);
                    double prevLng = ToRadians(prev.Longitude);
                    double lat = ToRadians(point.Latitude);
                    double lng = ToRadians(point.Longitude);
                    length += DistanceRadians(prevLat, prevLng, lat, lng);
                }
                prev = point;
            }
            return length * EarthRadius;
        }

        public static double ComputeArea(List<LatLng> path)
        {
            return Abs(ComputeSignedArea(path));
        }

        public static double ComputeSignedArea(List<LatLng> path)
        {
            return ComputeSignedArea(path, EarthRadius);
        }

        static double ComputeSignedArea(List<LatLng> path, double radius)
        {
            int size = path.Count;
            if (size < 3)
            {
                return 0;
            }
            double total = 0;
            LatLng prev = path[size - 1];
            double prevTanLat = Tan((PI / 2 - ToRadians(prev.Latitude)) / 2);
            double prevLng = ToRadians(prev.Longitude);
            for (int i = 0; i < size; i++)
            {
                LatLng point = path[i];
                double tanLat = Tan((PI / 2 - ToRadians(point.Latitude)) / 2);
                double lng = ToRadians(point.Longitude);
                total += PolarTriangleArea(tanLat, lng, prevTanLat, prevLng);
                prevTanLat = tanLat;
                prevLng = lng;
            }
            return total * (radius * radius);
        }

        private static double PolarTriangleArea(double tan1, double lng1, double tan2, double lng2)
        {
            double deltaLng = lng1 - lng2;
            double t = tan1 * tan2;
            return 2 * Atan2(t * Sin(deltaLng), 1 + t * Cos(deltaLng));
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
