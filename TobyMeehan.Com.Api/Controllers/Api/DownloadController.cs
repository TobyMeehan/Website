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
using TobyMeehan.Com.Api.Models.Api;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Api.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : OAuthControllerBase
    {
        private readonly IDownloadRepository _downloads;
        private readonly IMapper _mapper;

        public DownloadController(IDownloadRepository downloads, IMapper mapper)
        {
            _downloads = downloads;
            _mapper = mapper;
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
        [Authorize]
        public async Task<IActionResult> Post(DownloadRequest request)
        {
            if (!HasScope(Scopes.Downloads))
            {
                return Forbid(new ErrorResponse("The 'downloads' scope is required to create a download."));
            }

            var download = await _downloads.AddAsync(request.Title, request.ShortDescription, request.LongDescription, UserId);

            foreach (var author in request.Authors.Where(x => x.Id != UserId))
            {
                await _downloads.AddAuthorAsync(author.Id, UserId);
            }

            return Created(Url.Action(nameof(Get), new { download.Id }), download);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (!HasScope(Scopes.Downloads))
            {
                return Forbid(new ErrorResponse("The 'downloads' scope is required to delete a download."));
            }

            await _downloads.DeleteAsync(id);

            return NoContent();
        }
    }
}