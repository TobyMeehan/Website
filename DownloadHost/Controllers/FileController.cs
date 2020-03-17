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
        public IActionResult Get(string download, string file)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", download, file);

            return PhysicalFile(path, System.Net.Mime.MediaTypeNames.Application.Octet);
        }

        [Route("/{download}/{file}")]
        [HttpDelete]
        public IActionResult Delete(string download, string file)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", download, file);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            return Ok();
        }
    }
}