using System.Windows;
using Frontend.ViewModels;

namespace Frontend.Views
{
    public partial class ReviewDetailWindow : Window
    {
        public ReviewDetailWindow(ReviewDetailViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.CloseRequested += Close;
        }
    }
}