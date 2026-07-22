using NUnit.Framework;
using PetWalk.ViewModels;

namespace PetWalk.Tests
{
    [TestFixture]
    public class LoginViewModelTests
    {
        private LoginViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new LoginViewModel();
        }

        [Test]
        public void LoginCommand_WithEmptyEmail_ShouldNotExecute()
        {
            _viewModel.Email = "";
            _viewModel.Password = "password123";

            Assert.That(_viewModel.LoginCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void LoginCommand_WithEmptyPassword_ShouldNotExecute()
        {
            _viewModel.Email = "test@test.com";
            _viewModel.Password = "";

            Assert.That(_viewModel.LoginCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void LoginCommand_WithBothFields_ShouldBeExecutable()
        {
            _viewModel.Email = "test@test.com";
            _viewModel.Password = "password123";

            Assert.That(_viewModel.LoginCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void PropertyChanged_ShouldFireForEmail()
        {
            bool fired = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(LoginViewModel.Email))
                    fired = true;
            };

            _viewModel.Email = "new@email.com";

            Assert.That(fired, Is.True);
        }

        [Test]
        public void PropertyChanged_ShouldFireForErrorMessage()
        {
            bool fired = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(LoginViewModel.ErrorMessage))
                    fired = true;
            };

            _viewModel.ErrorMessage = "Test error";

            Assert.That(fired, Is.True);
        }
    }
}