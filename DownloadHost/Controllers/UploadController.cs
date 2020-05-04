using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using DownloadHost.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DownloadHost.Controllers
{
    public class UploadController : Controller
    {
        const int MaxFileSize = 200 * 1024 * 1024; // 200MB
        const int BufferSize = 512 * 1024; // 500KB#

        private readonly IFileManager _fileManager;

        public UploadController(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        [HttpPost("/upload")]
        [RequestSizeLimit(MaxFileSize)]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            string downloadId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string randomName = User.Identity.Name;

            if (!int.TryParse(User.Claims.First(c => c.Type == "Partitions").Value, out int partitions))
            {
                return BadRequest("Could not find partition claim.");
            }

            if (!Enumerable.Range(1, partitions).Any(x => file.FileName == $"{randomName}.part.{x}"))
            {
                return BadRequest("Invalid filename.");
            }

            await _fileManager.SaveFile(downloadId, randomName, file.OpenReadStream());

            

            if (int.TryParse(new string(file.FileName.Where(x => char.IsDigit(x)).ToArray()), out int index))
            {
                if (index == partitions)
                {
                    _fileManager.ZipUpFile(downloadId, randomName);
                }
            }

            return Created($"/{downloadId}/{file.FileName}", file.FileName);
        }
    }
}