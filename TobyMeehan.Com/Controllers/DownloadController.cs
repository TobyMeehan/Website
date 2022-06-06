using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            var dl = await _downloads.GetByIdAsync(download);

            if (dl.Visibility is DownloadVisibility.Private)
            {
                return Redirect($"/downloads/{download}");
            }

            var file = (await _files.GetByDownloadAndFilenameAsync(download, filename)).First();

            await _files.DownloadAsync(file.Id, HttpContext.Response.Body);

            return File(HttpContext.Response.Body, MediaTypeNames.Application.Octet, file.Filename,
                enableRangeProcessing: true);
        }
    }
}