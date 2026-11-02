using System.Windows.Input;
using Frontend.Services;

namespace Frontend.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            ToggleThemeCommand = new RelayCommand(_ =>
            {
                ThemeService.Toggle();
                OnPropertyChanged(nameof(ThemeButtonText));
            });
        }

        public ICommand ToggleThemeCommand { get; }

        public string ThemeButtonText => ThemeService.IsDark ? "Világos téma" : "Sötét téma";
    }
}
