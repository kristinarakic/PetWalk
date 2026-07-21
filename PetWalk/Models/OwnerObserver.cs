using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Models
{
    public class OwnerObserver : IObserver
    {
        private Owner _owner;
        public string LastNotification { get; private set; } = string.Empty;

        public OwnerObserver(Owner owner)
        {
            _owner = owner;
        }

        public void Update(string message)
        {
            LastNotification = message;
        }
    }
}
