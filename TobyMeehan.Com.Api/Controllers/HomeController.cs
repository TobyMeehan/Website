using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TobyMeehan.Com.Api.Models;

namespace TobyMeehan.Com.Api.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public IActionResult Index()
        {
            return Redirect("https://tobymeehan.com/developer");
        }
    }
}
