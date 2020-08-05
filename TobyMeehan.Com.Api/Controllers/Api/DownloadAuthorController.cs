using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Api.Authorization;
using TobyMeehan.Com.Api.Models.Api;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Api.Controllers.Api
{
    [Route("api")]
    [ApiController]
    public class DownloadAuthorController : OAuthControllerBase
    {
        private readonly IDownloadRepository _downloads;
        private readonly IUserRepository _users;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public DownloadAuthorController(IDownloadRepository downloads, IUserRepository users, IMapper mapper, IAuthorizationService authorizationService)
        {
            _downloads = downloads;
            _users = users;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet("users/{userid}/downloads")]
        [Authorize]
        public async Task<IActionResult> GetDownloads(string userid)
        {
            if (userid == "@me")
            {
                userid = UserId;
            }

            var user = await _users.GetByIdAsync(userid);

            if (user == null)
            {
                return NotFound(new ErrorResponse("The user does not exist."));
            }

            var downloads = await _downloads.GetByAuthorAsync(userid);

            return Ok(_mapper.Map<List<DownloadResponse>>(downloads));
        }

        [HttpDelete("users/{userid}/downloads/{downloadid}")]
        [Authorize]
        [Scope("downloads")]
        public async Task<IActionResult> Delete(string userid, string downloadid)
        {
            if (userid == "@me")
            {
                userid = UserId;
            }

            var user = await _users.GetByIdAsync(userid);

            if (user == null)
            {
                return NotFound(new ErrorResponse("The user does not exist."));
            }

            var download = await _downloads.GetByIdAsync(downloadid);

            if (download == null)
            {
                return NotFound(new ErrorResponse("The download does not exist."));
            }

            await _downloads.RemoveAuthorAsync(downloadid, userid);

            return NoContent();
        }

        [HttpGet("downloads/{id}/authors")]
        [Authorize]
        public async Task<IActionResult> GetAuthors(string id)
        {
            var download = await _downloads.GetByIdAsync(id);

            if (download == null)
            {
                return NotFound(new ErrorResponse("The download does not exist."));
            }

            return Ok(_mapper.Map<List<PartialUserResponse>>(download.Authors));
        }
    }
}