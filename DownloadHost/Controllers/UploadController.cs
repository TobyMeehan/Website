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
        public async Task<IActionResult> Index(string download, string filename, [FromBody]string content)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", download);
            Directory.CreateDirectory(path);
            path = Path.Combine(path, filename);

            byte[] data = Convert.FromBase64String(content);

            using var writer = System.IO.File.OpenWrite(path);
            await writer.WriteAsync(data);
            return Created($"/{download}/{filename}", filename);
        }
    }
}