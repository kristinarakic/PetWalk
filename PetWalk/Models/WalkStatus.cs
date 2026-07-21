using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Models
{
    public enum WalkStatus
    {
        Scheduled,
        Accepted,
        Declined,
        InProgress,
        Completed,
        Cancelled
    }
}
