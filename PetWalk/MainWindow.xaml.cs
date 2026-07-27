using System;
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

            try
            {
                using (var context = new PetWalkDbContext())
                {
                    context.Database.EnsureCreated();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Database Error");
            }

            ShowLoginView();
        }

        private void ShowLoginView()
        {
            try
            {
                var loginView = new LoginView();
                var loginViewModel = new LoginViewModel();

                loginViewModel.LoginSuccessful += OnLoginSuccessful;
                loginViewModel.NavigateToRegister += ShowRegistrationView;

                loginView.DataContext = loginViewModel;
                MainContent.Content = loginView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Error");
            }
        }

        private void ShowRegistrationView()
        {
            try
            {
                var registrationView = new RegistrationView();
                var registrationViewModel = new RegistrationViewModel();

                registrationViewModel.NavigateToLogin += ShowLoginView;
                registrationViewModel.RegistrationSuccessful += OnLoginSuccessful;

                registrationView.DataContext = registrationViewModel;
                MainContent.Content = registrationView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Error");
            }
        }

        private void OnLoginSuccessful(User user)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Error");
            }
        }

        private void ShowOwnerDashboard(Owner owner)
        {
            try
            {
                var view = new OwnerDashboardView();
                var viewModel = new OwnerDashboardViewModel(owner);

                viewModel.LogoutRequested += ShowLoginView;

                view.DataContext = viewModel;
                MainContent.Content = view;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Error");
            }
        }

        private void ShowWalkerDashboard(Walker walker)
        {
            try
            {
                var view = new WalkerDashboardView();
                var viewModel = new WalkerDashboardViewModel(walker);

                viewModel.LogoutRequested += ShowLoginView;

                view.DataContext = viewModel;
                MainContent.Content = view;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Error");
            }
        }
    }
}