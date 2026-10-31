using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using EatAndRate.Shared.DTOs.Auth;
using Frontend.Services;

namespace Frontend.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _usernameOrEmail = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public LoginViewModel()
        {
            LoginCommand = new AsyncRelayCommand(_ => LoginAsync(), _ => !IsBusy,
                ex => ErrorMessage = $"Váratlan hiba történt: {ex.Message}");
            GoToRegisterCommand = new RelayCommand(_ => RegisterRequested?.Invoke());
        }

        public event Action? LoginSucceeded;
        public event Action? RegisterRequested;

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public string UsernameOrEmail
        {
            get => _usernameOrEmail;
            set { if (SetProperty(ref _usernameOrEmail, value)) ErrorMessage = string.Empty; }
        }

        public string Password
        {
            get => _password;
            set { if (SetProperty(ref _password, value)) ErrorMessage = string.Empty; }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (SetProperty(ref _errorMessage, value))
                    OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                    RelayCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task LoginAsync()
        {
            var dto = new LoginDto
            {
                UsernameOrEmail = UsernameOrEmail.Trim(),
                Password = Password
            };

            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, new ValidationContext(dto), results, true))
            {
                ErrorMessage = string.Join("\n", results.Select(r => r.ErrorMessage));
                return;
            }

            IsBusy = true;
            try
            {
                var auth = await ApiClient.LoginAsync(dto);
                Session.Start(auth);
                LoginSucceeded?.Invoke();
            }
            catch (ApiException ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
