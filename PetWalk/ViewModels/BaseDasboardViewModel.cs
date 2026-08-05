using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PetWalk.Helpers;
using PetWalk.Models;
using PetWalk.Services;

namespace PetWalk.ViewModels
{
    public abstract class BaseDashboardViewModel : BaseViewModel
    {
        protected readonly SerializationService _serializationService;
        protected readonly ReportService _reportService;
        protected readonly WalkService _walkService;

        private string _profileFirstName = string.Empty;
        private string _profileLastName = string.Empty;
        private string _profilePhone = string.Empty;
        private string _profileLocation = string.Empty;
        private string _statusMessage = string.Empty;

        public BaseDashboardViewModel()
        {
            _serializationService = new SerializationService();
            _reportService = new ReportService();
            _walkService = new WalkService();

            SaveProfileCommand = new RelayCommand(ExecuteSaveProfile);
            ExportJsonCommand = new RelayCommand(_ => ExportToJson());
            ExportXmlCommand = new RelayCommand(_ => ExportToXml());
            GenerateReportCommand = new RelayCommand(_ => GenerateReport());
            LogoutCommand = new RelayCommand(_ => OnLogout());
        }

        public string ProfileFirstName
        {
            get => _profileFirstName;
            set => SetProperty(ref _profileFirstName, value);
        }

        public string ProfileLastName
        {
            get => _profileLastName;
            set => SetProperty(ref _profileLastName, value);
        }

        public string ProfilePhone
        {
            get => _profilePhone;
            set => SetProperty(ref _profilePhone, value);
        }

        public string ProfileLocation
        {
            get => _profileLocation;
            set => SetProperty(ref _profileLocation, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public abstract string WelcomeMessage { get; }

        public ICommand SaveProfileCommand { get; }
        public ICommand ExportJsonCommand { get; }
        public ICommand ExportXmlCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand LogoutCommand { get; }

        public event Action? LogoutRequested;

        protected void LoadProfile(User user)
        {
            ProfileFirstName = user.FirstName;
            ProfileLastName = user.LastName;
            ProfilePhone = user.Phone;
            ProfileLocation = user.Location;
        }

        protected abstract void ExecuteSaveProfile(object? parameter);
        protected abstract void ExportToJson();
        protected abstract void ExportToXml();
        protected abstract void GenerateReport();

        protected void OnLogout()
        {
            AuthService.GetInstance().Logout();
            LogoutRequested?.Invoke();
        }
    }
}