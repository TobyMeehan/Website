using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com
{
    public class AlertState : IEnumerable<AlertModel>
    {
        private List<AlertModel> _alerts = new List<AlertModel>();

        public void Add(AlertModel alert)
        {
            _alerts.Add(alert);
            StateHasChanged();
        }

        public void Dismiss(AlertModel alert)
        {
            _alerts.Remove(alert);
        }

        private void StateHasChanged() => OnStateChanged?.Invoke();

        public IEnumerator<AlertModel> GetEnumerator()
        {
            return _alerts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event Action OnStateChanged;
    }
}
