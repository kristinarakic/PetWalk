    using PetWalk.Helpers;
using PetWalk.Models;
using PetWalk.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PetWalk.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _phone = string.Empty;
        private bool _isOwner = true;
        private string _location = string.Empty;
        private decimal _hourlyRate;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

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

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public bool IsOwner
        {
            get => _isOwner;
            set
            {
                SetProperty(ref _isOwner, value);
                OnPropertyChanged(nameof(IsWalker));
                OnPropertyChanged(nameof(ShowWalkerFields));
            }
        }

        public bool IsWalker
        {
            get => !_isOwner;
            set => IsOwner = !value;
        }

        public bool ShowWalkerFields => !IsOwner;

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public decimal HourlyRate
        {
            get => _hourlyRate;
            set => SetProperty(ref _hourlyRate, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public event Action? NavigateToLogin;
        public event Action<User>? RegistrationSuccessful;

        public RegistrationViewModel()
        {
            RegisterCommand = new RelayCommand(ExecuteRegister, CanExecuteRegister);
            GoToLoginCommand = new RelayCommand(_ => NavigateToLogin?.Invoke());
        }

        private bool CanExecuteRegister(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteRegister(object? parameter)
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            User newUser;

            if (IsOwner)
            {
                newUser = new Owner
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    Phone = Phone
                };
            }
            else
            {
                newUser = new Walker
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    Phone = Phone,
                    Location = Location,
                    HourlyRate = HourlyRate,
                    IsAvailable = true
                };
            }

            var authService = AuthService.GetInstance();
            bool success = authService.Register(newUser);

            if (success)
            {
                var loggedIn = authService.Login(newUser.Email, newUser.Password);
                if (loggedIn != null)
                {
                    RegistrationSuccessful?.Invoke(loggedIn);
                }
            }
            else
            {
                ErrorMessage = "Email already exists.";
            }
        }
    }
}
