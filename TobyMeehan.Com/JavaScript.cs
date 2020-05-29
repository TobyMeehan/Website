using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com
{
    public class JavaScript
    {
        private readonly IJSRuntime _jSRuntime;

        public JavaScript(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task ShowTab(string selector)
        {
            await _jSRuntime.InvokeVoidAsync(nameof(ShowTab), selector);
        }
        public Task ShowUsernameTab() => ShowTab("#ChangeUsernameTab");
        public Task ShowPasswordTab() => ShowTab("#ChangePasswordTab");
        public Task ShowAccountTab() => ShowTab("#DeleteAccountTab");
    }
}
