using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorUI
{
    public class ErrorModel : PageModel
    {
        public IActionResult OnGet(int error_id)
        {
            Error error = (Error)error_id;

            Message = error switch
            {
                Error.NotFound => "Could not find an application with the provided client ID.",
                Error.InvalidCredentials => "One or more of the provided credentials for the application were invalid, for example the redirect URL may not have matched the one which was registered."
            };

            return Page();
        }

        public string Message { get; set; }
    }
}