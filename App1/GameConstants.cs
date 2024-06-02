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
    class GameConstants
    {
        public const double TowerShellDamageRadius =20;
        public const double TowerShellDamage = 20;
        public const int PlayerHealth = 100;
        public const int PlayerRoundsPerMinute = 30;
        public const int TowerRoundsPerMinute = 10;
        public const double PlayerShellDamageRadius=20;
        
        public const double towerAimingTime = 0.1;
        public const double towerAngleChangeStep = 2;
    }
}