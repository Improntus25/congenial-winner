using System;
using System.Windows;
using ModernNesting.UI;

namespace ModernNesting
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var application = new Application();
            application.StartupUri = new Uri("UI/MainWindow.xaml", UriKind.Relative);
            application.Run();
        }
    }
}
