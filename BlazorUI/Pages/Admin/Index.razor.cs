using AutoMapper;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Pages.Admin
{
    public partial class Index : ComponentBase
    {
        [Inject] private IUserProcessor userProcessor { get; set; }
        [Inject] private IRoleProcessor roleProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }

        private List<Role> _roles;
        private List<User> _users;

        private SingleTextFormModel _roleForm = new SingleTextFormModel();

        protected override async Task OnInitializedAsync()
        {
            _users = await Task.Run(async () => mapper.Map<List<User>>(await userProcessor.GetUsers()));
            _roles = await Task.Run(async () => mapper.Map<List<Role>>(await roleProcessor.GetRoles()));
        }

        private async Task RoleForm_Submit()
        {
            await roleProcessor.CreateRole(_roleForm.Value);
            _roles.Add(mapper.Map<Role>(await roleProcessor.GetRoleByName(_roleForm.Value)));
        }

        private async Task RoleButton_Click(Role role)
        {
            await roleProcessor.DeleteRole(mapper.Map<DataAccessLibrary.Models.Role>(role));
            _roles.Remove(role);
        }
    }
}
