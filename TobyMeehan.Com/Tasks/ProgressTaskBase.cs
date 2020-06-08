using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Tasks
{
    public abstract class ProgressTaskBase : IProgressTask
    {
        protected Func<CancellationToken, Task> TaskSource;
        protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public int PercentageProgress { get; protected set; }

        public TaskStatus Status { get; protected set; }

        protected void NotifyProgressChanged() => OnProgress?.Invoke(this);
        public event Action<IProgressTask> OnProgress;
        protected void NotifyComplete() => OnComplete?.Invoke(this);
        public event Action<IProgressTask> OnComplete;

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
        }

        public async Task Start()
        {
            Status = TaskStatus.InProgress;

            try
            {
                await TaskSource(CancellationTokenSource.Token);
                Status = TaskStatus.Completed;
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
            {
                Status = TaskStatus.Cancelled;
                return;
            }
            catch
            {
                Status = TaskStatus.Failed;
                return;
            }
            finally
            {
                NotifyComplete();
            }
        }
    }
}
