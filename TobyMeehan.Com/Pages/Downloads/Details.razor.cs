using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Details : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }
        [Inject] private IUserRepository users { get; set; }
        [Inject] private ICommentRepository comments { get; set; }
        [Inject] private AlertState alertState { get; set; }

        [Parameter] public string Id { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private Download _download;
        private User _user;
        private IEnumerable<Comment> _comments;
        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _download = await Task.Run(() => downloads.GetByIdAsync(Id));
            _comments = await Task.Run(() => comments.GetByEntityAsync(Id));
            _context = await AuthenticationStateTask;

            if (_context.User.Identity.IsAuthenticated)
            {
                _user = await Task.Run(() => users.GetByIdAsync(_context.User.Id()));
            }
        }

        private async Task VerifyForm_Submit()
        {
            await downloads.UpdateAsync(_download);

            alertState.Add(new AlertModel { Context = BootstrapContext.Success, ChildContent = VerifyAlertContent(_download.Verified) });
        }

        private async Task CommentForm_Submit(CommentViewModel comment)
        {
            await comments.AddAsync(_download.Id, _user.Id, comment.Content);
            _comments = await Task.Run(() => comments.GetByEntityAsync(Id));
        }
    }
}
