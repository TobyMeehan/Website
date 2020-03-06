using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DownloadHost.Controllers
{
    public class FileController : Controller
    {
        [Route("/{download}/{file}")]
        public IActionResult Index(string download, string file)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", download, file);

            return PhysicalFile(path, System.Net.Mime.MediaTypeNames.Application.Octet);
        }
    }
}