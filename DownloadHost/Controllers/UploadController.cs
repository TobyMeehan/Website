using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DownloadHost.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        [Route("/upload/{download}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Index(string download, IFormFile file)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", download);
            Directory.CreateDirectory(path);
            path = Path.Combine(path, file.FileName);

            using (var stream = System.IO.File.Create(path))
            {
                await file.CopyToAsync(stream);
            }

            return Created($"/{download}/{file.FileName}", file.FileName);
        }
    }
}