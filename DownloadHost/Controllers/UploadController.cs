using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DownloadHost.Controllers
{    
    public class UploadController : Controller
    {
        [HttpPost]
        [Route("/upload/{download}/{filename}")]
        public async Task<IActionResult> Index([FromQuery]string download,[FromQuery]string filename)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", download, filename);

            using var writer = System.IO.File.OpenWrite(path);
            await Request.Body.CopyToAsync(writer);

            return Ok();
        }
    }
}