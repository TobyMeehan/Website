using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DownloadHost.Models;

namespace DownloadHost.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("https://tobymeehan.com/downloads");
        }
    }
}
