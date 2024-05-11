using System;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using System.Text.Json;

namespace CttApp
{
    public static class Config
    {
        public static string GetApiKey()
        {
            try
            {
                PackageManager manager = Application.Context.PackageManager;
                string packageName = Application.Context.PackageName;
                Bundle metaData = manager.GetApplicationInfo(packageName, PackageInfoFlags.MetaData).MetaData;

                if (metaData != null)
                {
                    return metaData.GetString("com.google.android.geo.API_KEY");
                }
            }
            catch (PackageManager.NameNotFoundException e)
            {
                // Handle the error, possibly log it
                Console.WriteLine("Error: " + e.Message);
            }
            return string.Empty;
        }
    }

    public class ElevationService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;

        public ElevationService()
        {
            httpClient = new HttpClient();
            apiKey = Config.GetApiKey(); // Retrieve API key securely
        }

        public async Task<double?> GetElevationAsync(double latitude, double longitude)
        {
            string requestUri = $"https://maps.googleapis.com/maps/api/elevation/json?locations={latitude},{longitude}&key={apiKey}";

            HttpResponseMessage response = await httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                try
                {
                    var elevationResult = JsonSerializer.Deserialize<ElevationResponse>(json);
                    return elevationResult?.results[0]?.elevation;
                }
                catch (JsonException e)
                {
                    Console.WriteLine($"JSON Parse Error: {e.Message}");
                }
            }
            else
            {
                Console.WriteLine($"HTTP Error: {response.StatusCode}");
            }
            return null;
        }

        public class ElevationResponse
        {
            public ElevationResult[] results { get; set; }
        }

        public class ElevationResult
        {
            public double elevation { get; set; }
        }
    }
}
