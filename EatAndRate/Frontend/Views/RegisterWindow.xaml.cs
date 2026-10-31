using System.Windows;
using System.Windows.Controls;
using Frontend.ViewModels;

namespace Frontend.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _viewModel = new();

        public RegisterWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;

            _viewModel.RegisterSucceeded += () =>
            {
                new OverViewWindow().Show(); // valahogy maskepp kene
                Close();
            };
            _viewModel.LoginRequested += () =>
            {
                new LoginWindow().Show();
                Close();
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = ((PasswordBox)sender).Password;
        }
    }
}
