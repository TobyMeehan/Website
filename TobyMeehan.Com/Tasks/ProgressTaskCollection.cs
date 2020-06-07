using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Tasks
{
    public class ProgressTaskCollection : ICollection<IProgressTask>
    {
        public ProgressTaskCollection()
        {
            ItemAdded += Item_OnAdded;
        }

        private List<IProgressTask> _items = new List<IProgressTask>();

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public void Add(IProgressTask item)
        {
            int index = _items.FindIndex(x => !(x.Status == TaskStatus.InProgress || x.Status == TaskStatus.Queued));
            index = index < 0 ? _items.Count : index;

            _items.Insert(index, item);

            item.OnComplete += Item_OnComplete;

            ItemAdded?.Invoke();
        }

        private async void Item_OnComplete(IProgressTask task)
        {
            _items.Remove(task);
            _items.Add(task);

            if (_items.First().Status == TaskStatus.Queued)
            {
                await _items.First().Start();
            }
        }

        private event Action ItemAdded;
        private async void Item_OnAdded()
        {
            if (_items.First().Status == TaskStatus.Queued)
            {
                await _items.First().Start();
            }
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(IProgressTask item) => _items.Contains(item);

        public void CopyTo(IProgressTask[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public IEnumerator<IProgressTask> GetEnumerator() => _items.GetEnumerator();

        public bool Remove(IProgressTask item)
        {
            if (item.Status == TaskStatus.InProgress)
            {
                item.Cancel();
                return true;
            }
            else
            {
                return _items.Remove(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
