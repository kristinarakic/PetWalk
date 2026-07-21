using PetWalk.Helpers;
using PetWalk.Models;
using PetWalk.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PetWalk.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public event Action<User>? LoginSuccessful;
        public event Action? NavigateToRegister;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            GoToRegisterCommand = new RelayCommand(_ => NavigateToRegister?.Invoke());
        }

        private bool CanExecuteLogin(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object? parameter)
        {
            ErrorMessage = string.Empty;

            var authService = AuthService.GetInstance();
            var user = authService.Login(Email, Password);

            if (user == null)
            {
                ErrorMessage = "Invalid email or password.";
                return;
            }

            LoginSuccessful?.Invoke(user);
        }
    }
}
