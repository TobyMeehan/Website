using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Components;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Upload;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;
using TobyMeehan.Com.Tasks;

namespace TobyMeehan.Com.Pages.Settings
{
    public partial class Index : ComponentBase
    {
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject] private IUserRepository users { get; set; }
        [Inject] private NavigationManager navigation { get; set; }
        [Inject] private AlertState alertState { get; set; }

        [CascadingParameter] public User CurrentUser { get; set; }

        private ServerSideValidator _usernameValidator;
        private UsernameViewModel _usernameModel = new UsernameViewModel();
        private async Task UsernameForm_Submit()
        {
            string username = _usernameModel.Username;

            if (await users.AnyUsernameAsync(username))
            {
                _usernameValidator.Error(nameof(_usernameModel.Username), "That username is already in use.");

                return;
            }

            await users.UpdateUsernameAsync(CurrentUser.Id, _usernameModel.Username);

            navigation.NavigateToRefresh();
        }

        private ServerSideValidator _urlValidator;
        private VanityUrlViewModel _urlModel = new VanityUrlViewModel();
        private async Task UrlForm_Submit()
        {
            if (string.IsNullOrWhiteSpace(_urlModel.VanityUrl))
            {
                await users.UpdateVanityUrlAsync(CurrentUser.Id, null);
                CurrentUser.VanityUrl = null;
                return;
            }

            if (await users.AnyVanityUrlAsync(_urlModel.VanityUrl))
            {
                _urlValidator.Error(nameof(_urlModel), "That URL is unavailable.");

                return;
            }

            await users.UpdateVanityUrlAsync(CurrentUser.Id, _urlModel.VanityUrl);

            CurrentUser.VanityUrl = _urlModel.VanityUrl;
            _urlModel.VanityUrl = "";
        }

        private ServerSideValidator _passwordValidator;
        private ChangePasswordViewModel _passwordModel = new ChangePasswordViewModel();
        private async Task PasswordForm_Submit()
        {
            if (!(await users.AuthenticateAsync(CurrentUser.Username, _passwordModel.CurrentPassword)).Success)
            {
                _passwordValidator.Error(nameof(_passwordModel.CurrentPassword), "Incorrect current password.");

                return;
            }

            await users.UpdatePasswordAysnc(CurrentUser.Id, _passwordModel.NewPassword);

            _passwordModel = new ChangePasswordViewModel();

            alertState.Add(new AlertModel
            {
                Context = BootstrapContext.Success,
                ChildContent = _alertContent
            });
        }

        private ServerSideValidator _deleteValidator;
        private PasswordViewModel _deleteModel = new PasswordViewModel();
        private async Task DeleteForm_Submit()
        {
            if (!(await users.AuthenticateAsync(CurrentUser.Username, _deleteModel.Password)).Success)
            {
                _deleteValidator.Error(nameof(_deleteModel.Password), "Incorrect password.");

                return;
            }

            await users.DeleteAsync(CurrentUser.Id);

            navigation.NavigateToLogout();
        }

        private readonly int _maxSize = 10 * 1024 * 1024;
        private async Task ProfilePicture_Change(IEnumerable<IFileReference> files)
        {
            IFileReference file = files.FirstOrDefault();

            if (file == null)
            {
                return;
            }

            var fileInfo = await file.ReadFileInfoAsync();

            if (fileInfo.Size > _maxSize)
            {
                return;
            }

            using (Stream uploadStream = await file.OpenReadAsync())
            {
                await users.AddProfilePictureAsync(CurrentUser.Id, fileInfo.Name, fileInfo.Type, uploadStream);
            }

            navigation.NavigateTo("/me", true);
        }

        private async Task RemoveProfilePicture()
        {
            await users.RemoveProfilePictureAsync(CurrentUser.Id);
            CurrentUser.ProfilePictureUrl = null;
        }

        private async Task DescriptionForm_Submit()
        {
            await users.UpdateDescriptionAsync(CurrentUser.Id, CurrentUser.Description);
        }
    }
}
