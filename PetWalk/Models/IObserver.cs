using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Models
{
    public interface IObserver
    {
        void Update(string message);
    }
}
