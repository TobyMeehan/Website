using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Api.Models.Api;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Api.Controllers.Api
{
    [Route("api/applications/@me/scoreboard")]
    [ApiController]
    public class ScoreboardController : OAuthControllerBase
    {
        private readonly IScoreboardRepository _scoreboard;
        private readonly IUserRepository _users;
        private readonly IMapper _mapper;

        public ScoreboardController(IScoreboardRepository scoreboard, IUserRepository users, IMapper mapper)
        {
            _scoreboard = scoreboard;
            _users = users;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var scoreboard = await _scoreboard.GetByApplicationAsync(AppId);

            return Ok(_mapper.Map<List<ObjectiveResponse>>(scoreboard));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            var objective = (await _scoreboard.GetByApplicationAsync(AppId)).FirstOrDefault(o => o.Id == id);

            if (objective == null)
            {
                return NotFound(new ErrorResponse("The objective does not exist."));
            }

            return Ok(_mapper.Map<ObjectiveResponse>(objective));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(ObjectiveRequest request)
        {
            var objective = await _scoreboard.AddAsync(AppId, request.Name);

            return Created(Url.Action(nameof(Get)), _mapper.Map<ObjectiveResponse>(objective));
        }

        [HttpDelete("{id")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var objective = (await _scoreboard.GetByApplicationAsync(AppId)).FirstOrDefault(o => o.Id == id);

            if (objective == null)
            {
                return NotFound(new ErrorResponse("The objective does not exist."));
            }

            await _scoreboard.DeleteAsync(id);

            return NoContent();
        }

        [HttpPut("/{id}/users/{userid}")]
        [Authorize]
        public async Task<IActionResult> PutScore(string id, string userid, ScoreRequest request)
        {
            if (userid == "@me")
            {
                userid = UserId;
            }

            var objective = (await _scoreboard.GetByApplicationAsync(AppId)).FirstOrDefault(o => o.Id == id);

            if (objective == null)
            {
                return NotFound(new ErrorResponse("The objective does not exist."));
            }

            var user = await _users.GetByIdAsync(userid);

            if (user == null)
            {
                return NotFound(new ErrorResponse("The user does not exist."));
            }

            await _scoreboard.SetScoreAsync(id, userid, request.Value);

            return NoContent();
        }
    }
}