using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DownloadHost.Controllers
{
    public class FileController : Controller
    {
        [Route("/{download}/{filename}")]
        public async Task<IActionResult> Get(string download, string filename)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", download);

            if (System.IO.File.Exists(Path.Combine(path, filename)))
            {
                return PhysicalFile(Path.Combine(path, filename), MediaTypeNames.Application.Octet);
            }

            List<string> files = Directory.GetFiles(path, $"{filename}.part.*").OrderBy(x => x).ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                foreach (string file in files)
                {
                    using (Stream stream = System.IO.File.OpenRead(file))
                    {
                        await stream.CopyToAsync(ms);
                    }
                }

                return File(ms.ToArray(), MediaTypeNames.Application.Octet);
            }
        }

        [Route("/{download}/{filename}")]
        [HttpDelete]
        public IActionResult Delete(string download, string filename)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", download);

            if (Directory.Exists(path))
            {
                if (System.IO.File.Exists(Path.Combine(path, filename)))
                {
                    System.IO.File.Delete(Path.Combine(path, filename));
                }

                List<string> files = Directory.GetFiles(path, $"{filename}.part.*").ToList();

                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                }
            }

            return Ok();
        }
    }
}