using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Authentication;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Components.Accounts
{
    public partial class ChangePasswordForm : ComponentBase
    {
        [Parameter] public EventCallback<ChangePasswordViewModel> OnValidSubmit { get; set; }

        private ChangePasswordViewModel _model = new ChangePasswordViewModel();

        private ServerSideValidator _serverSideValidator;

        private Task Form_ValidSubmit()
        {
            return OnValidSubmit.InvokeAsync(_model);
        }

        public void InvalidCurrentPassword(params string[] messages) => _serverSideValidator.Error(nameof(_model.CurrentPassword), messages);
        public void InvalidNewPassword(params string[] messages) => _serverSideValidator.Error(nameof(_model.NewPassword), messages);
    }
}
