using NUnit.Framework;
using PetWalk.Models;

namespace PetWalk.Tests
{
    [TestFixture]
    public class WalkTests
    {
        [Test]
        public void Walk_DefaultStatus_ShouldBeScheduled()
        {
            var walk = new Walk();

            Assert.That(walk.Status, Is.EqualTo(WalkStatus.Scheduled));
        }

        [Test]
        public void Walk_ChangeStatus_ShouldUpdateStatus()
        {
            var walk = new Walk();

            walk.ChangeStatus(WalkStatus.Accepted);

            Assert.That(walk.Status, Is.EqualTo(WalkStatus.Accepted));
        }

        [Test]
        public void Walk_ChangeStatus_ShouldNotifyObservers()
        {
            var walk = new Walk();
            var owner = new Owner { FirstName = "Test", LastName = "Owner" };
            var observer = new OwnerObserver(owner);
            walk.Attach(observer);

            walk.ChangeStatus(WalkStatus.Accepted);

            Assert.That(observer.LastNotification, Does.Contain("Accepted"));
        }

        [Test]
        public void Walk_DetachObserver_ShouldNotReceiveNotification()
        {
            var walk = new Walk();
            var owner = new Owner { FirstName = "Test", LastName = "Owner" };
            var observer = new OwnerObserver(owner);
            walk.Attach(observer);
            walk.Detach(observer);

            walk.ChangeStatus(WalkStatus.Completed);

            Assert.That(observer.LastNotification, Is.EqualTo(string.Empty));
        }
    }
}