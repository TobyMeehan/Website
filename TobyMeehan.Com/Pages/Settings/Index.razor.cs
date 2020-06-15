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
using TobyMeehan.Com.Components.Accounts;
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

        private AccountSettingsForm _form;

        private async Task UsernameForm_Submit(UsernameViewModel model)
        {
            string username = model.Username;

            if (await users.AnyUsernameAsync(username))
            {
                _form.UsernameForm.InvalidUsername("That username is already in use.");

                return;
            }

            await users.UpdateUsernameAsync(CurrentUser.Id, model.Username);

            navigation.NavigateToRefresh();
        }

        private async Task PasswordForm_Submit(ChangePasswordViewModel model)
        {
            if (!(await users.AuthenticateAsync(CurrentUser.Username, model.CurrentPassword)).Success)
            {
                _form.PasswordForm.InvalidCurrentPassword("Incorrect current password.");

                return;
            }

            await users.UpdatePasswordAysnc(CurrentUser.Id, model.NewPassword);

            model = new ChangePasswordViewModel();

            alertState.Add(new AlertModel
            {
                Context = BootstrapContext.Success,
                ChildContent = _alertContent
            });
        }

        private async Task AccountForm_Submit(PasswordViewModel model)
        {
            if (!(await users.AuthenticateAsync(CurrentUser.Username, model.Password)).Success)
            {
                _form.AccountForm.InvalidPassword("Incorrect password.");

                return;
            }

            await users.DeleteAsync(CurrentUser.Id);

            navigation.NavigateTo("/logout", true);
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

            Refresh();
        }

        private async Task RemoveProfilePicture()
        {
            await users.RemoveProfilePictureAsync(CurrentUser.Id);
            Refresh();
        }

        private void Refresh()
        {
            navigation.NavigateTo("/me", true);
        }
    }
}
