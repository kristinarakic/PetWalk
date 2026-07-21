using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetWalk.Models
{
    public class Walk : ISubject
    {
        [Key]
        public int Id { get; set; }

        public int OwnerId { get; set; }
        public int WalkerId { get; set; }
        public int DogId { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        public int Duration { get; set; }

        public WalkStatus Status { get; set; } = WalkStatus.Scheduled;

        public decimal Price { get; set; }

        [ForeignKey("OwnerId")]
        public Owner? Owner { get; set; }

        [ForeignKey("WalkerId")]
        public Walker? Walker { get; set; }

        [ForeignKey("DogId")]
        public Dog? Dog { get; set; }

        public Review? Review { get; set; }

        [NotMapped]
        private List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            string message = $"Walk status changed to {Status} for {ScheduledDate:dd.MM.yyyy HH:mm}";
            foreach (var observer in _observers)
            {
                observer.Update(message);
            }
        }

        public void ChangeStatus(WalkStatus newStatus)
        {
            Status = newStatus;
            Notify();
        }
    }
}
