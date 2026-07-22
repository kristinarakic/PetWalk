using Microsoft.EntityFrameworkCore;
using PetWalk.Data;
using PetWalk.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Repositories
{
    public class DogRepository : IRepository<Dog>
    {
        private readonly PetWalkDbContext _context;

        public DogRepository(PetWalkDbContext context)
        {
            _context = context;
        }

        public List<Dog> GetAll()
        {
            return _context.Dogs
                .Include(d => d.Owner)
                .ToList();
        }

        public Dog? GetById(int id)
        {
            return _context.Dogs
                .Include(d => d.Owner)
                .FirstOrDefault(d => d.Id == id);
        }

        public void Add(Dog entity)
        {
            _context.Dogs.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Dog entity)
        {
            _context.Dogs.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var dog = _context.Dogs.Find(id);
            if (dog != null)
            {
                _context.Dogs.Remove(dog);
                _context.SaveChanges();
            }
        }

        public List<Dog> GetByOwnerId(int ownerId)
        {
            return _context.Dogs
                .Where(d => d.OwnerId == ownerId)
                .ToList();
        }
    }
}
