using System.Windows;
using System.Windows.Controls;
using Frontend.ViewModels;

namespace Frontend.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }
    }
}
