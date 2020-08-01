// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.ShowTab = (selector) => {
    $(selector).tab('show');
}

window.SetTitle = (title) => {
    document.title = title;
}

window.CopyToClipboard = (text) => {
    navigator.clipboard.writeText(text);
}