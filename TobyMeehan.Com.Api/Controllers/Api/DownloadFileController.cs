using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using TobyMeehan.Com.Api.Authorization;
using TobyMeehan.Com.Api.Models;
using TobyMeehan.Com.Api.Models.Api;
using TobyMeehan.Com.AspNetCore.Authorization;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Api.Controllers.Api
{
    [Route("api/downloads/{id}/files")]
    [ApiController]
    public class DownloadFileController : OAuthControllerBase
    {
        private readonly IDownloadRepository _downloads;
        private readonly IDownloadFileRepository _files;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public DownloadFileController(IDownloadRepository downloads, IDownloadFileRepository files, IMapper mapper, IAuthorizationService authorizationService)
        {
            _downloads = downloads;
            _files = files;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            var download = await _downloads.GetByIdAsync(id);

            if (download == null)
            {
                return NotFound(new ErrorResponse("The download does not exist."));
            }

            return Ok(_mapper.Map<List<DownloadFileResponse>>(download.Files));
        }

        //[HttpGet("{fileid}")]
        //[Authorize]
        //public async Task<IActionResult> Get(string id, string fileid)
        //{
        //    var download = await _downloads.GetByIdAsync(id);

        //    if (download == null)
        //    {
        //        return NotFound(new ErrorResponse("The download does not exist."));
        //    }

        //    if (!download.Files.TryGetItem(fileid, out DownloadFile file))
        //    {
        //        return NotFound(new ErrorResponse("The file does not exist."));
        //    }

        //    return Ok(_mapper.Map<DownloadFileResponse>(file));
        //}

        [HttpPost]
        [Authorize(ScopePolicies.HasDownloadsScope)]
        public async Task<IActionResult> Post(string id, DownloadFileRequest request, bool? overwriteExisting)
        {
            var download = await _downloads.GetByIdAsync(id);

            if (download == null)
            {
                return NotFound("The download does not exist.");
            }

            var result = await _authorizationService.AuthorizeAsync(User, download, new UpdateOperationAuthorizationRequirement());

            if (!result.Succeeded)
            {
                return Forbid(new ErrorResponse("Not authorized to upload file."));
            }

            var existingFiles = download.Files.Where(f => f.Filename == request.Filename);

            if (existingFiles.Any() && (overwriteExisting ?? false))
            {
                foreach (var existingFile in existingFiles)
                {
                    await _files.DeleteAsync(existingFile.Id);
                }
            }

            var file = await _files.AddAsync(id, request.Filename, request.File.OpenReadStream());

            return Created($"{Url.Action(nameof(Get))}/{file.Id}", file);
        }

        //[HttpDelete("{fileid}")]
        //[Authorize]
        //[Scope("downloads")]
        //public async Task<IActionResult> Delete(string id, string fileid)
        //{
        //    var download = _mapper.Map<DownloadModel>(await _downloads.GetByIdAsync(id));

        //    if (download == null)
        //    {
        //        return NotFound(new ErrorResponse("The download does not exist."));
        //    }

        //    var result = await _authorizationService.AuthorizeAsync(User, download, new AuthorizationRequirement(Operation.Delete));

        //    if (!result.Succeeded)
        //    {
        //        return Forbid(result.Failure);
        //    }

        //    if (!download.Files.TryGetItem(fileid, out DownloadFileModel file))
        //    {
        //        return NotFound(new ErrorResponse("The file does not exist."));
        //    }

        //    await _files.DeleteAsync(fileid);

        //    return NoContent();
        //}
    }
}