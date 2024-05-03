using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CttApp
{
    class Utils
    {
        public static double Deg2Rad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}