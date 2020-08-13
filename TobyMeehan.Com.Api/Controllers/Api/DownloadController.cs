using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    [Route("api/downloads")]
    [ApiController]
    public class DownloadController : OAuthControllerBase
    {
        private readonly IDownloadRepository _downloads;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public DownloadController(IDownloadRepository downloads, IMapper mapper, IAuthorizationService authorizationService)
        {
            _downloads = downloads;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var downloads = await _downloads.GetAsync();

            return Ok(_mapper.Map<List<DownloadResponse>>(downloads));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            var download = await _downloads.GetByIdAsync(id);

            if (download == null)
            {
                return NotFound(new ErrorResponse("The download does not exist."));
            }

            return Ok(_mapper.Map<DownloadResponse>(download));
        }

        [HttpPost]
        [Authorize(ScopePolicies.HasDownloadsScope, Roles = "Verified")]
        public async Task<IActionResult> Post(DownloadRequest request)
        {
            if (!Version.TryParse(request.Version, out Version version))
            {
                return BadRequest(new ErrorResponse("Invalid version string."));
            }

            var download = await _downloads.AddAsync(request.Title, request.ShortDescription, request.LongDescription, version, UserId);

            return Created(Url.Action(nameof(Get), new { download.Id }), download);
        }

        [HttpPut("{id}")]
        [Authorize(ScopePolicies.HasDownloadsScope)]
        public async Task<IActionResult> Put(string id, DownloadRequest request)
        {
            var download = await _downloads.GetByIdAsync(id);

            if (download == null)
            {
                return NotFound(new ErrorResponse("The download does not exist."));
            }

            var result = await _authorizationService.AuthorizeAsync(User, download, new UpdateOperationAuthorizationRequirement());

            if (!result.Succeeded)
            {
                return Forbid(new ErrorResponse("Not authorized to modify download."));
            }

            if (!Version.TryParse(request.Version, out Version version))
            {
                return BadRequest(new ErrorResponse("Invalid version string."));
            }

            var updated = await _downloads.UpdateAsync(id, _mapper.Map<Download>(request));

            return Ok(_mapper.Map<DownloadResponse>(updated));
        }

        [HttpDelete("{id}")]
        [Authorize(ScopePolicies.HasDownloadsScope)]
        public async Task<IActionResult> Delete(string id)
        {
            var download = await _downloads.GetByIdAsync(id);

            if (download == null)
            {
                return NotFound(new ErrorResponse("The download does not exist."));
            }

            var result = await _authorizationService.AuthorizeAsync(User, download, new DeleteOperationAuthorizationRequirement());

            if (!result.Succeeded)
            {
                return Forbid(new ErrorResponse("Not authorized to delete download."));
            }

            await _downloads.DeleteAsync(id);

            return NoContent();
        }
    }
}