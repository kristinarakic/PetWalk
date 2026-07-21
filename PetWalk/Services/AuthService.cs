using PetWalk.Data;
using PetWalk.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Services
{
    public class AuthService
    {
        private static AuthService? _instance;
        private static readonly object _lock = new object();

        private User? _currentUser;
        private readonly UserRepository _userRepository;

        private AuthService()
        {
            var context = new PetWalkDbContext();
            _userRepository = new UserRepository(context);
        }

        public static AuthService GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AuthService();
                    }
                }
            }
            return _instance;
        }

        public User? Login(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);

            if (user == null)
                return null;

            if (user.Password != password)
                return null;

            _currentUser = user;
            return user;
        }

        public bool Register(User user)
        {
            var existing = _userRepository.GetByEmail(user.Email);
            if (existing != null)
                return false;

            user.RegistrationDate = DateTime.Now;
            _userRepository.Add(user);
            return true;
        }

        public User? GetCurrentUser()
        {
            return _currentUser;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public static void ResetInstance()
        {
            _instance = null;
        }
    }
}
