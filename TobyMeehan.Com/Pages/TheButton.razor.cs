using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Pages
{
    public partial class TheButton : ComponentBase, IDisposable
    {
        [Inject] private IButtonRepository presses { get; set; }

        private IEnumerable<ButtonPress> _previousPresses = new List<ButtonPress>();

        private Timer _timer;
        private int _secondsElapsed;

        protected override async Task OnInitializedAsync()
        {
            _previousPresses = await Task.Run(presses.GetAsync);

            _secondsElapsed = _previousPresses.OrderByDescending(p => p.TimePressed).FirstOrDefault()?.ButtonSeconds ?? 0;
            SetTimer();
        }

        private int GetPercentageProgress()
        {
            double totalSeconds = TimeSpan.FromMinutes(1).TotalSeconds;

            return (int)(((double)_secondsElapsed / totalSeconds) * 100d);
        }

        private void SetTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _secondsElapsed++;
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnButtonClick(string userId)
        {
            await presses.AddAsync(userId, TimeSpan.FromSeconds(_secondsElapsed));
            _secondsElapsed = 0;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
