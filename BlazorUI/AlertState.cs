using BlazorUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI
{
    public class AlertState
    {
        public AlertState()
        {
            Queue.CollectionChanged += QueueChanged;
        }
        public ObservableCollection<Alert> Queue { get; set; } = new ObservableCollection<Alert>();

        public event Action OnChange;

        private void QueueChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
