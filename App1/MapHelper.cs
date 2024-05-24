using Xamarin.Essentials;
using System;

public static class MapHelper
{
    // Earth's circumference at the equator in meters
    private const double EarthCircumference = 40075000;

    public static float EstimateZoomLevel(double radiusMeters)
    {
        // Get the screen width in pixels
        var screenWidth = 2341.95; // calculated manually for s24//DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;


        // Assume the radius should cover half the screen width
        double metersPerPixel = radiusMeters / (screenWidth / 2);

        // Calculate the zoom level
        double zoomLevel = Math.Log(EarthCircumference / (256 * metersPerPixel)) / Math.Log(2);
        return (float)Math.Floor(zoomLevel);  // Floor to get the closest lower whole number
    }
}