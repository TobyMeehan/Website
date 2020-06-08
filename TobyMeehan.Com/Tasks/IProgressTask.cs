using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Tasks
{
    public interface IProgressTask
    {
        Task Start();

        void Cancel();

        int PercentageProgress { get; }

        TaskStatus Status { get; }

        event Action<IProgressTask> OnProgress;
        event Action<IProgressTask> OnComplete;
    }
}
