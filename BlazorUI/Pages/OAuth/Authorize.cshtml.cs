using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorUI.Extensions;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorUI
{
    [Authorize]
    public class AuthorizeModel : PageModel
    {
        private readonly IApplicationProcessor _applicationProcessor;
        private readonly IConnectionProcessor _connectionProcessor;
        private readonly IMapper _mapper;

        public AuthorizeModel(IApplicationProcessor applicationProcessor, IConnectionProcessor connectionProcessor, IMapper mapper, AlertState alertState)
        {
            _applicationProcessor = applicationProcessor;
            _connectionProcessor = connectionProcessor;
            _mapper = mapper;
        }

        public Application Application { get; set; }

        public async Task<IActionResult> OnGet(string client_id, string redirect_uri, string state, string code_challenge)
        {
            Application = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(client_id));

            if (Application == null)
            {
                return RedirectToPage("Error", new { error_id = (int)Error.NotFound });
            }

            if (Application.RedirectUri != redirect_uri)
            {
                return RedirectToPage("Error", new { error_id = (int)Error.InvalidCredentials });
            }

            if ((await _connectionProcessor.GetConnectionByUserAndApplication(User.GetUserId(), Application.Id)) != null)
            {
                return await AuthorizationCode(Application.Id, Application.RedirectUri, state, code_challenge);
            }
            else
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnPost(string client_id, string redirect_uri, string state, string code_challenge)
        {
            return await AuthorizationCode(client_id, redirect_uri, state, code_challenge);
        }

        private async Task<IActionResult> AuthorizationCode(string clientId, string redirectUri, string state, string codeChallenge)
        {
            string code = (await _connectionProcessor.CreateAuthorizationCode(User.GetUserId(), clientId)).Code;

            if (codeChallenge != null)
            {
                await _connectionProcessor.CreatePkce(new DataAccessLibrary.Models.Pkce { ClientId = clientId, CodeChallenge = codeChallenge });
            }

            return Redirect($"{redirectUri}?code={code}&state={state}");
        }
    }
}