using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Extensions;

namespace TobyMeehan.Com.Pages.Settings
{
    public partial class Transactions : ComponentBase
    {
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [CascadingParameter] public User CurrentUser { get; set; }
    }
}
