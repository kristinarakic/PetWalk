using Microsoft.EntityFrameworkCore;
using PetWalk.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Data
{
    public class UserRepository : IRepository<User>
    {
        private readonly PetWalkDbContext _context;

        public UserRepository(PetWalkDbContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User? GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public void Add(User entity)
        {
            _context.Users.Add(entity);
            _context.SaveChanges();
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public List<Walker> GetAllWalkers()
        {
            return _context.Walkers
                .Include(w => w.Reviews)
                .ToList();
        }

        public List<Walker> GetAvailableWalkers()
        {
            return _context.Walkers
                .Include(w => w.Reviews)
                .Where(w => w.IsAvailable)
                .ToList();
        }
    }
}
