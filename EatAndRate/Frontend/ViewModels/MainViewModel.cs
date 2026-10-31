using System;
using System.Collections.Generic;
using System.Text;
using Frontend.Services;

namespace Frontend.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel = null!;
        

        public MainViewModel() 
        {
            ShowRegister();
        }
        public ViewModelBase CurrentViewModel 
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        private void ShowRegister()
        {
            var vm = new RegisterViewModel();
            vm.RegisterSucceeded += ShowOverview;
            vm.LoginRequested += ShowLogin;
            CurrentViewModel = vm;
        }

        private void ShowLogin()
        {
            var vm = new LoginViewModel();
            vm.LoginSucceeded += ShowOverview;
            vm.RegisterRequested += ShowRegister;
            CurrentViewModel = vm;
        }

        private void ShowOverview()
        {
            var vm = new OverviewViewModel();
            vm.LogoutRequested += () =>
            {
                Session.End();
                ShowLogin();
            };
            CurrentViewModel = vm;
        }
    }
}
