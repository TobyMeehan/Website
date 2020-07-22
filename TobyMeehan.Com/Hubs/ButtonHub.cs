using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Hubs
{
    public class ButtonHub : Hub
    {
        private readonly IButtonRepository _repository;

        public ButtonHub(IButtonRepository repository)
        {
            _repository = repository;
        }

        public async Task PressButton(string userId, int timeSpan)
        {
            await _repository.AddAsync(userId, TimeSpan.FromSeconds(timeSpan));
            await Clients.All.SendAsync("ButtonPressed");
        }
    }
}
