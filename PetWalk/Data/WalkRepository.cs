using Microsoft.EntityFrameworkCore;
using PetWalk.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Data
{
    public class WalkRepository : IRepository<Walk>
    {
        private readonly PetWalkDbContext _context;

        public WalkRepository(PetWalkDbContext context)
        {
            _context = context;
        }

        public List<Walk> GetAll()
        {
            return _context.Walks
                .Include(w => w.Owner)
                .Include(w => w.Walker)
                .Include(w => w.Dog)
                .Include(w => w.Review)
                .ToList();
        }

        public Walk? GetById(int id)
        {
            return _context.Walks
                .Include(w => w.Owner)
                .Include(w => w.Walker)
                .Include(w => w.Dog)
                .Include(w => w.Review)
                .FirstOrDefault(w => w.Id == id);
        }

        public void Add(Walk entity)
        {
            _context.Walks.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Walk entity)
        {
            _context.Walks.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var walk = _context.Walks.Find(id);
            if (walk != null)
            {
                _context.Walks.Remove(walk);
                _context.SaveChanges();
            }
        }

        public List<Walk> GetByOwnerId(int ownerId)
        {
            return _context.Walks
                .Include(w => w.Walker)
                .Include(w => w.Dog)
                .Include(w => w.Review)
                .Where(w => w.OwnerId == ownerId)
                .OrderByDescending(w => w.ScheduledDate)
                .ToList();
        }

        public List<Walk> GetByWalkerId(int walkerId)
        {
            return _context.Walks
                .Include(w => w.Owner)
                .Include(w => w.Dog)
                .Include(w => w.Review)
                .Where(w => w.WalkerId == walkerId)
                .OrderByDescending(w => w.ScheduledDate)
                .ToList();
        }

        public List<Walk> GetScheduledByWalkerId(int walkerId)
        {
            return _context.Walks
                .Include(w => w.Owner)
                .Include(w => w.Dog)
                .Where(w => w.WalkerId == walkerId && w.Status == WalkStatus.Scheduled)
                .OrderBy(w => w.ScheduledDate)
                .ToList();
        }
    }
}
