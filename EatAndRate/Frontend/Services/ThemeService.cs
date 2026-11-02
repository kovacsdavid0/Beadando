using System.Windows;

namespace Frontend.Services
{
    public static class ThemeService
    {
        private static readonly Uri LightTheme = new("pack://application:,,,/Styles/LightTheme.xaml");
        private static readonly Uri DarkTheme = new("pack://application:,,,/Styles/DarkTheme.xaml");

        public static bool IsDark { get; private set; }

        public static void Toggle()
        {
            IsDark = !IsDark;

            Application.Current.Resources.MergedDictionaries[0] =
                new ResourceDictionary { Source = IsDark ? DarkTheme : LightTheme };
        }
    }
}
