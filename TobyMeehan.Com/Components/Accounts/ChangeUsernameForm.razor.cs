using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Components.Accounts
{
    public partial class ChangeUsernameForm : ComponentBase
    {
        [Inject] private IRepository<User> users { get; set; }

        [Parameter] public string Username { get; set; }
        [Parameter] public EventCallback<string> OnValidSubmit { get; set; }

        private UsernameViewModel _model = new UsernameViewModel();

        private ServerSideValidator _serverSideValidator;

        protected override void OnInitialized()
        {
            _model.Username = Username;
        }

        private async Task Form_ValidSubmit()
        {
            if (await users.AnyAsync(u => u.Username == _model.Username))
            {
                _serverSideValidator.Error(nameof(_model.Username), "That username is already in use.");

                return;
            }

            await OnValidSubmit.InvokeAsync(_model.Username);
        }
    }
}
