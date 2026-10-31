using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using EatAndRate.Shared.DTOs.Auth;
using Frontend.Services;

namespace Frontend.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(async _ => await RegisterAsync(), _ => !IsBusy);
            GoToLoginCommand = new RelayCommand(_ => LoginRequested?.Invoke());
        }

        public event Action? RegisterSucceeded;
        public event Action? LoginRequested;

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public string Username
        {
            get => _username;
            set { if (SetProperty(ref _username, value)) ErrorMessage = string.Empty; }
        }

        public string Email
        {
            get => _email;
            set { if (SetProperty(ref _email, value)) ErrorMessage = string.Empty; }
        }

        // A PasswordBox nem bindolható, ezt a View állítja be a PasswordChanged eseményben
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

        private async Task RegisterAsync()
        {
            var dto = new RegisterDto
            {
                Username = Username.Trim(),
                Email = Email.Trim(),
                Password = Password
            };

            // Ugyanazok a szabályok, mint az API-n (a Shared DTO attribútumai)
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, new ValidationContext(dto), results, true))
            {
                ErrorMessage = string.Join("\n", results.Select(r => r.ErrorMessage));
                return;
            }

            if (dto.Username.Contains('@'))
            {
                ErrorMessage = "A felhasználónév nem tartalmazhat @ karaktert.";
                return;
            }

            IsBusy = true;
            try
            {
                var auth = await ApiClient.RegisterAsync(dto);
                Session.Start(auth);
                RegisterSucceeded?.Invoke();
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
