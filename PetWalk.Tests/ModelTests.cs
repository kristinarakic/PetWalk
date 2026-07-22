using NUnit.Framework;
using PetWalk.Models;

namespace PetWalk.Tests
{
    [TestFixture]
    public class ModelTests
    {
        [Test]
        public void User_GetFullName_ShouldReturnConcatenatedName()
        {
            var owner = new Owner { FirstName = "John", LastName = "Doe" };

            string fullName = owner.GetFullName();

            Assert.That(fullName, Is.EqualTo("John Doe"));
        }

        [Test]
        public void Walker_CalculateAverageRating_WithNoReviews_ShouldReturnZero()
        {
            var walker = new Walker();

            double result = walker.CalculateAverageRating();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Walker_CalculateAverageRating_WithReviews_ShouldReturnAverage()
        {
            var walker = new Walker();
            walker.Reviews.Add(new Review { Rating = 4 });
            walker.Reviews.Add(new Review { Rating = 5 });
            walker.Reviews.Add(new Review { Rating = 3 });

            double result = walker.CalculateAverageRating();

            Assert.That(result, Is.EqualTo(4.0));
        }

        [Test]
        public void Owner_AddDog_ShouldAddToList()
        {
            var owner = new Owner { Id = 1 };
            var dog = new Dog { Name = "Rex" };

            owner.AddDog(dog);

            Assert.That(owner.Dogs.Count, Is.EqualTo(1));
            Assert.That(dog.OwnerId, Is.EqualTo(1));
        }

        [Test]
        public void Owner_RemoveDog_ShouldRemoveFromList()
        {
            var owner = new Owner();
            var dog = new Dog { Id = 1, Name = "Rex" };
            owner.Dogs.Add(dog);

            owner.RemoveDog(1);

            Assert.That(owner.Dogs.Count, Is.EqualTo(0));
        }
    }
}