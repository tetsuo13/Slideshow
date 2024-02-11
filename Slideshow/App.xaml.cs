using System.Windows;

namespace Slideshow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                dragNdropArg = e.Args[0];
            }
            else
            {
                dragNdropArg = string.Empty;
            }
        }

        public static string dragNdropArg;
    }
}
