using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorUI.Extensions;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorUI
{
    [AutoValidateAntiforgeryToken]
    public class AuthModel : PageModel
    {
        private readonly IApplicationProcessor _applicationProcessor;
        private readonly IConnectionProcessor _connectionProcessor;
        private readonly IMapper _mapper;
        private readonly AlertState _alertState;

        public AuthModel(IApplicationProcessor applicationProcessor, IConnectionProcessor connectionProcessor, IMapper mapper, AlertState alertState)
        {
            _applicationProcessor = applicationProcessor;
            _connectionProcessor = connectionProcessor;
            _mapper = mapper;
            _alertState = alertState;
        }

        public Application Application { get; set; }

        private async Task<IActionResult> Authorize(string clientId, string redirectUri, string state)
        {
            string code = (await _connectionProcessor.CreateAuthorizationCode(User.GetUserId(), clientId)).Code;

            return Redirect($"{redirectUri}?code={code}&state={state}");
        }

        public async Task<IActionResult> OnGet(string client_id, string redirect_uri, string state)
        {
            Application = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(client_id));

            if (Application == null)
            {
                _alertState.Queue.Add(new Alert
                {
                    Title = "Oh no...",
                    Body = $"Failed to authorize {Application.Name} - client id not found.",
                    Context = AlertContext.Danger
                });
                return LocalRedirect("/");
            }

            if (Application.RedirectUri != redirect_uri)
            {
                _alertState.Queue.Add(new Alert
                {
                    Title = "Oh no...",
                    Body = $"Failed to authorize {Application.Name} - redirect URIs do not match.",
                    Context = AlertContext.Danger
                });
                return LocalRedirect("/");
            }

            if (User.Identity.IsAuthenticated)
            {
                if ((await _connectionProcessor.GetConnectionByUserAndApplication(User.GetUserId(), client_id)) != null) // if there is already a connection, the user has already authorised the application
                {
                    return await Authorize(client_id, redirect_uri, state);
                }

                return Page();
            }

            return LocalRedirect("/login?redirectUri=/auth");
        }

        public async Task<IActionResult> OnPost(string client_id, string redirect_uri, string state)
        {
            return await Authorize(client_id, redirect_uri, state);
        }
    }
}