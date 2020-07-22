using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
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
        [Inject] private NavigationManager navigation { get; set; }

        private IEnumerable<ButtonPress> _previousPresses = new List<ButtonPress>();

        private HubConnection _hubConnection;

        private Timer _timer;
        private int _secondsElapsed;
        private int _percentageProgress => presses.GetButtonPercentage(_secondsElapsed);
        private int _clickCount;

        protected override async Task OnInitializedAsync()
        {
            await ConfigureHubConnection();

            _previousPresses = await Task.Run(presses.GetAsync);
            _clickCount = _previousPresses.Count();

            if (_previousPresses.Any())
            {
                _secondsElapsed = (int)(DateTime.Now - _previousPresses.Max(p => p.TimePressed)).TotalSeconds;
            }
            else
            {
                _secondsElapsed = 0;
            }

            SetTimer();
        }

        private async Task ConfigureHubConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigation.ToAbsoluteUri("/buttonhub"))
                .Build();

            _hubConnection.On("ButtonPressed", () =>
            {
                _secondsElapsed = 0;
                _clickCount++;
            });

            await _hubConnection.StartAsync();
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
            await _hubConnection.SendAsync("PressButton", userId, _secondsElapsed);
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _ = _hubConnection.DisposeAsync();
        }
    }
}
