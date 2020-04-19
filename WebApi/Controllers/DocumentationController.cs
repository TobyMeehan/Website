using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("/Docs")]
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}