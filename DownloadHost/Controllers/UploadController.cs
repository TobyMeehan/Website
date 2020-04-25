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
        const int MaxFileSize = 200 * 1024 * 1024; // 200MB
        const int BufferSize = 512 * 1024; // 500KB

        [HttpPost("/upload/{download}")]
        [RequestSizeLimit(MaxFileSize)]
        public async Task<IActionResult> Upload(string download, IFormFile file)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", download);
            Directory.CreateDirectory(path);

            const int maxPartitions = MaxFileSize / BufferSize;
            string filename = file.FileName;

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

            return Created($"/{download}/{file.FileName}", file.FileName);
        }
    }
}