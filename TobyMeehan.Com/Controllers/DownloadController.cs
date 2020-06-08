using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IDownloadFileRepository _files;

        public DownloadController(IDownloadFileRepository files)
        {
            _files = files;
        }

        [HttpGet("/downloads/{download}/{filename}")]
        public async Task<IActionResult> Download(string download, string filename)
        {
            DownloadFile file = (await _files.GetByDownloadAndFilenameAsync(download, filename)).First();

            using (MemoryStream ms = new MemoryStream())
            {
                await _files.DownloadAsync(file.Id, ms);
                return File(ms.ToArray(), MediaTypeNames.Application.Octet);
            }
        }
    }
}