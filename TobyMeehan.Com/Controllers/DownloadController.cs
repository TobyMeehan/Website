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
        private readonly IDownloadRepository _downloads;
        private readonly IDownloadFileRepository _files;

        public DownloadController(IDownloadRepository downloads, IDownloadFileRepository files)
        {
            _downloads = downloads;
            _files = files;
        }

        [HttpGet("/downloads/{download}/file/{filename}")]
        public async Task<IActionResult> Download(string download, string filename)
        {
            Download dl = await _downloads.GetByIdAsync(download);

            if (dl.Visibility == DownloadVisibility.Private)
            {
                return Redirect($"/downloads/{download}");
            }

            DownloadFile file = (await _files.GetByDownloadAndFilenameAsync(download, filename)).First();

            return Redirect(file.Url);
        }
    }
}