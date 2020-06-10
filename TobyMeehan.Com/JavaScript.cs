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

        public async Task InitCKEditor<T>(string id, DotNetObjectReference<T> dotNetReference) where T : class
        {
            await _jSRuntime.InvokeVoidAsync("CKEditor.init", id, dotNetReference);
        }

        public ValueTask DestroyCKEditor(string id)
        {
            return _jSRuntime.InvokeVoidAsync("CKEditor.destroy", id);
        }

        public ValueTask SetTitle(string title)
        {
            return _jSRuntime.InvokeVoidAsync(nameof(SetTitle), title);
        }
    }
}
