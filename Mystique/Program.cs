using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inscribe.Configuration;
using System.Windows;

namespace Mystique
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            // 設定のロード
            Setting.Initialize();

            if (Setting.Instance.ExperienceProperty.ShowSplashScreen)
            {
                new SplashScreen("resources/krile2_splash.png").Show(true);
            }

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
