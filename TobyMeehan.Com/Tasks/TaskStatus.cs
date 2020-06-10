using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Tasks
{
    public enum TaskStatus
    {
        Queued,
        InProgress,
        Failed,
        Cancelled,
        Completed
    }
}
