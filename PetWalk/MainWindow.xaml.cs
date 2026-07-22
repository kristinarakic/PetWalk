using System.Windows;
using PetWalk.Data;
using PetWalk.Models;
using PetWalk.ViewModels;
using PetWalk.Views;

namespace PetWalk
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using (var context = new PetWalkDbContext())
            {
                context.Database.EnsureCreated();
            }

            ShowLoginView();
        }

        private void ShowLoginView()
        {
            var loginView = new LoginView();
            var loginViewModel = new LoginViewModel();

            loginViewModel.LoginSuccessful += OnLoginSuccessful;
            loginViewModel.NavigateToRegister += ShowRegistrationView;

            loginView.DataContext = loginViewModel;
            MainContent.Content = loginView;
        }

        private void ShowRegistrationView()
        {
            var registrationView = new RegistrationView();
            var registrationViewModel = new RegistrationViewModel();

            registrationViewModel.NavigateToLogin += ShowLoginView;

            registrationView.DataContext = registrationViewModel;
            MainContent.Content = registrationView;
        }

        private void OnLoginSuccessful(User user)
        {
            if (user is Owner owner)
            {
                ShowOwnerDashboard(owner);
            }
            else if (user is Walker walker)
            {
                ShowWalkerDashboard(walker);
            }
        }

        private void ShowOwnerDashboard(Owner owner)
        {
            var view = new OwnerDashboardView();
            var viewModel = new OwnerDashboardViewModel(owner);

            viewModel.LogoutRequested += ShowLoginView;

            view.DataContext = viewModel;
            MainContent.Content = view;
        }

        private void ShowWalkerDashboard(Walker walker)
        {
            var view = new WalkerDashboardView();
            var viewModel = new WalkerDashboardViewModel(walker);

            viewModel.LogoutRequested += ShowLoginView;

            view.DataContext = viewModel;
            MainContent.Content = view;
        }
    }
}