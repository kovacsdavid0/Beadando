using System.Windows;
using Frontend.ViewModels;

namespace Frontend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
