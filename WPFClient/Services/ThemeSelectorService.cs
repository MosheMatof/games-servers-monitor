using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ControlzEx.Theming;
using MahApps.Metro.Theming;

using WPFClient.Contracts.Services;
using WPFClient.Models;

namespace WPFClient.Services;

public class ThemeSelectorService : IThemeSelectorService
{
    private const string HcDarkTheme = "pack://application:,,,/Styles/Themes/HC.Dark.Blue.xaml";
    private const string HcLightTheme = "pack://application:,,,/Styles/Themes/HC.Light.Blue.xaml";

    public ThemeSelectorService()
    {
        ThemeManager.Current.ThemeChanged += ThemeManager_ThemeChanged;
    }

    public void InitializeTheme()
    {
        // TODO: Mahapps.Metro supports syncronization with high contrast but you have to provide custom high contrast themes
        // We've added basic high contrast dictionaries for Dark and Light themes
        // Please complete these themes following the docs on https://mahapps.com/docs/themes/thememanager#creating-custom-themes
        ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcDarkTheme), MahAppsLibraryThemeProvider.DefaultInstance));
        ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcLightTheme), MahAppsLibraryThemeProvider.DefaultInstance));

        var theme = GetCurrentTheme();
        SetTheme(theme);
    }

    public void SetTheme(AppTheme theme)
    {
        if (theme == AppTheme.Default)
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();
        }
        else
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithHighContrast;
            ThemeManager.Current.SyncTheme();
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.Blue", SystemParameters.HighContrast);
        }

        App.Current.Properties["Theme"] = theme.ToString();
    }

    public AppTheme GetCurrentTheme()
    {
        if (App.Current.Properties.Contains("Theme"))
        {
            var themeName = Application.Current.Properties["Theme"].ToString();
            Enum.TryParse(themeName, out AppTheme theme);
            return theme;
        }

        return AppTheme.Default;
    }

    private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
    {
        try
        {
            var window = Application.Current.MainWindow as Window;
            if (window != null)
            {
                if (e.NewTheme.BaseColorScheme == "Dark")
                {
                    var backgroundBrush = Application.Current.Resources["DarkBackgroundBrush"] as ImageBrush;
                    window.Background = backgroundBrush;
                }
                else
                {
                    var backgroundBrush = Application.Current.Resources["LightBackgroundBrush"] as ImageBrush;
                    window.Background = backgroundBrush;
                }
            }
        }
        catch
        {

        }
    }
}
