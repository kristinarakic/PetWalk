using PetWalk.Data;
using PetWalk.Models;
using PetWalk.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Services
{
    public class WalkService
    {
        private readonly WalkRepository _walkRepository;
        private readonly PetWalkDbContext _context;

        public WalkService()
        {
            _context = new PetWalkDbContext();
            _walkRepository = new WalkRepository(_context);
        }

        public WalkService(PetWalkDbContext context)
        {
            _context = context;
            _walkRepository = new WalkRepository(context);
        }

        public Walk ScheduleWalk(int ownerId, int walkerId, int dogId, DateTime scheduledDate, int duration, decimal price)
        {
            var walk = new Walk
            {
                OwnerId = ownerId,
                WalkerId = walkerId,
                DogId = dogId,
                ScheduledDate = scheduledDate,
                Duration = duration,
                Price = price,
                Status = WalkStatus.Scheduled
            };

            _walkRepository.Add(walk);
            return walk;
        }

        public void AcceptWalk(int walkId)
        {
            var walk = _walkRepository.GetById(walkId);
            if (walk != null)
            {
                walk.ChangeStatus(WalkStatus.Accepted);
                _walkRepository.Update(walk);
            }
        }

        public void DeclineWalk(int walkId)
        {
            var walk = _walkRepository.GetById(walkId);
            if (walk != null)
            {
                walk.ChangeStatus(WalkStatus.Declined);
                _walkRepository.Update(walk);
            }
        }

        public void StartWalk(int walkId)
        {
            var walk = _walkRepository.GetById(walkId);
            if (walk != null)
            {
                walk.ChangeStatus(WalkStatus.InProgress);
                _walkRepository.Update(walk);
            }
        }

        public void CompleteWalk(int walkId)
        {
            var walk = _walkRepository.GetById(walkId);
            if (walk != null)
            {
                walk.ChangeStatus(WalkStatus.Completed);
                _walkRepository.Update(walk);
            }
        }

        public void CancelWalk(int walkId)
        {
            var walk = _walkRepository.GetById(walkId);
            if (walk != null)
            {
                walk.ChangeStatus(WalkStatus.Cancelled);
                _walkRepository.Update(walk);
            }
        }

        public List<Walk> GetWalksByOwnerId(int ownerId)
        {
            return _walkRepository.GetByOwnerId(ownerId);
        }

        public List<Walk> GetWalksByWalkerId(int walkerId)
        {
            return _walkRepository.GetByWalkerId(walkerId);
        }

        public List<Walk> GetScheduledWalksByWalkerId(int walkerId)
        {
            return _walkRepository.GetScheduledByWalkerId(walkerId);
        }

        public Walk? GetWalkById(int id)
        {
            return _walkRepository.GetById(id);
        }
    }
}
