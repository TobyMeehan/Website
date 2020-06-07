using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Tasks
{
    public class ProgressTaskState
    {
        private ProgressTaskCollection _tasks = new ProgressTaskCollection();
        public IEnumerable<IProgressTask> Tasks => _tasks;

        private void StateHasChanged() => OnStateChanged?.Invoke();
        public event Action OnStateChanged;

        public void Add(IProgressTask task)
        {
            task.OnProgress += t => StateHasChanged();

            _tasks.Add(task);

            StateHasChanged();
        }

        public void Dismiss(IProgressTask task)
        {
            _tasks.Remove(task);

            StateHasChanged();
        }
    }
}
