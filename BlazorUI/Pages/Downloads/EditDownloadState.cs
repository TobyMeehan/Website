using System;

namespace BlazorUI.Pages.Downloads
{
    public class EditDownloadState
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set 
            { 
                _title = value;
                NotifyStateChanged();
            }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyStateChanged();
            }
        }

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
