using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Models.Api;

namespace WebApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ScoreboardController : AuthenticatedControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserProcessor _userProcessor;
        private readonly IApplicationProcessor _applicationProcessor;
        private readonly IScoreboardProcessor _scoreboardProcessor;

        public ScoreboardController(IMapper mapper, IUserProcessor userProcessor, IApplicationProcessor applicationProcessor, IScoreboardProcessor scoreboardProcessor) : base(userProcessor, mapper)
        {
            _mapper = mapper;
            _userProcessor = userProcessor;
            _applicationProcessor = applicationProcessor;
            _scoreboardProcessor = scoreboardProcessor;
        }

        [HttpGet]
        public async Task<ActionResult<Scoreboard>> Get()
        {
            return _mapper.Map<Scoreboard>((await _applicationProcessor.GetApplicationById(AppId)).Scoreboard);
        }

        [HttpPost("objective")]
        public async Task<ActionResult<Objective>> Post([FromBody] string name)
        {
            await _scoreboardProcessor.CreateObjective(new DataAccessLibrary.Models.Objective
            {
                Name = name,
                AppId = AppId
            });

            return Ok();
        }

        [HttpPost("score")]
        public async Task<IActionResult> Post(ScoreRequest request)
        {
            Application application = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(AppId));

            if (!application.Scoreboard.Objectives.Any(o => o.Id == request.Objective))
            {
                return NotFound();
            }

            await _scoreboardProcessor.SetScore(UserId, request.Objective, request.Score);

            return Ok();
        }

        [HttpDelete("objective")]
        public async Task<IActionResult> Delete([FromBody] string objective)
        {
            await _scoreboardProcessor.DeleteObjective(objective);

            return Ok();
        }
    }
}