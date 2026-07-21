using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Models
{
    public class Owner : User
    {
        public List<Dog> Dogs { get; set; } = new List<Dog>();
        public List<Walk> ScheduledWalks { get; set; } = new List<Walk>();
        public List<Review> WrittenReviews { get; set; } = new List<Review>();

        public Owner()
        {
            UserType = "Owner";
        }

        public void AddDog(Dog dog)
        {
            dog.OwnerId = this.Id;
            Dogs.Add(dog);
        }

        public void RemoveDog(int dogId)
        {
            var dog = Dogs.Find(d => d.Id == dogId);
            if (dog != null)
            {
                Dogs.Remove(dog);
            }
        }
    }
}
