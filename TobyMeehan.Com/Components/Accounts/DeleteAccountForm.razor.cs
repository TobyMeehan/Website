using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Components.Accounts
{
    public partial class DeleteAccountForm : ComponentBase
    {
        [Parameter] public EventCallback<PasswordViewModel> OnValidSubmit { get; set; }

        private PasswordViewModel _model = new PasswordViewModel();

        private ServerSideValidator _serverSideValidator;

        private Task Form_ValidSubmit()
        {
            return OnValidSubmit.InvokeAsync(_model);
        }

        public void InvalidPassword(params string[] messages) => _serverSideValidator.Error(nameof(_model.Password), messages);
    }
}
