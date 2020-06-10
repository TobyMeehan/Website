using System;
using System.Collections.Generic;
using System.Linq;
using TobyMeehan.Com.Tasks;

namespace TobyMeehan.Com.Extensions
{
    public static class ProgressTaskExtensions
    {
        public static bool IsComplete(this IProgressTask task)
        {
            return task.Status == TaskStatus.Completed ||
            task.Status == TaskStatus.Cancelled ||
            task.Status == TaskStatus.Failed;
        }
    }
}
