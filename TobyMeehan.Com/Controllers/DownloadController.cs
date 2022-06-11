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

        private async Task<IActionResult> ResultAsync(string download, string filename, bool inline = false)
        {
            var dl = await _downloads.GetByIdAsync(download);

            if (dl.Visibility is DownloadVisibility.Private)
            {
                return Redirect($"/downloads/{download}");
            }

            var stream = new MemoryStream();
            
            var file = (await _files.GetByDownloadAndFilenameAsync(download, filename)).First();

            await _files.DownloadAsync(file.Id, stream);
            
            HttpContext.Response.Headers.Add("Content-Disposition", $"{(inline ? "inline" : "attachment")};filename={file.Filename}");

            return File(stream.ToArray(), MimeTypes.GetMimeType(file.Filename), enableRangeProcessing: true);
        }

        [HttpGet("/downloads/{download}/file/{filename}")]
        public async Task<IActionResult> Download(string download, string filename)
        {
            return await ResultAsync(download, filename);
        }

        [HttpGet("/downloads/{download}/file/{filename}/inline")]
        public async Task<IActionResult> DownloadInline(string download, string filename)
        {
            var result = await ResultAsync(download, filename, inline: true);

            return result;
        }
    }
}