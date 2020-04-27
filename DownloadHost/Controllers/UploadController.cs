using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DownloadHost.Controllers
{
    public class UploadController : Controller
    {
        const int MaxFileSize = 200 * 1024 * 1024; // 200MB
        const int BufferSize = 512 * 1024; // 500KB

        [HttpPost("/upload/{download}")]
        [RequestSizeLimit(MaxFileSize)]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            string downloadId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", downloadId);
            Directory.CreateDirectory(path);

            const int maxPartitions = MaxFileSize / BufferSize;
            string filename = User.Identity.Name;

            if (!Enumerable.Range(1, maxPartitions).Any(x => filename.EndsWith($".part.{x}")))
            {
                filename += ".part.1";
            }

            path = Path.Combine(path, filename);

            using (var stream = System.IO.File.Create(path))
            {
                await file.CopyToAsync(stream);
                stream.Close();
            }

            return Created($"/{downloadId}/{file.FileName}", file.FileName);
        }
    }
}