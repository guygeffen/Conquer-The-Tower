using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using AndroidX.Core.Text;

namespace CttApp
{


    public class CustomInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        private readonly View infoWindow;

        public CustomInfoWindowAdapter(LayoutInflater inflater)
        {
            infoWindow = inflater.Inflate(Resource.Layout.custom_info_window, null);
        }

        public View GetInfoContents(Marker marker)
        {
            Render(marker, infoWindow);
            return infoWindow;
        }

        public View GetInfoWindow(Marker marker)
        {
            return null;
        }

        private void Render(Marker marker, View view)
        {
            var title = view.FindViewById<TextView>(Resource.Id.title);
            var snippet = view.FindViewById<TextView>(Resource.Id.snippet);

            if (title != null)
            {
                title.TextFormatted = HtmlCompat.FromHtml($"<b>{marker.Title}</b>", HtmlCompat.FromHtmlModeLegacy);
            }

            if (snippet != null)
            {
                snippet.TextFormatted = HtmlCompat.FromHtml(marker.Snippet, HtmlCompat.FromHtmlModeLegacy);
            }
        }
    }

}