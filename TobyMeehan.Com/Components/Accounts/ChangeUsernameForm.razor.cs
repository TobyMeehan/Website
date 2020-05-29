using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Components.Accounts
{
    public partial class ChangeUsernameForm : ComponentBase
    {
        [Parameter] public EventCallback<UsernameViewModel> OnValidSubmit { get; set; }

        private UsernameViewModel _model = new UsernameViewModel();

        private ServerSideValidator _serverSideValidator;

        private Task Form_ValidSubmit()
        {
            return OnValidSubmit.InvokeAsync(_model);
        }

        public void InvalidUsername(params string[] messages) => _serverSideValidator.Error(nameof(_model.Username), messages);
    }
}
