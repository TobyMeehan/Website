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
    [Route("api/applications")]
    [ApiController]
    public class ApplicationController : OAuthControllerBase
    {
        private readonly IApplicationRepository _applications;
        private readonly IMapper _mapper;

        public ApplicationController(IApplicationRepository applications, IMapper mapper)
        {
            _applications = applications;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            if (id == "@me")
            {
                id = AppId;
            }

            var application = await _applications.GetByIdAsync(id);

            if (application == null)
            {
                return NotFound(new ErrorResponse("The application does not exist."));
            }

            return Ok(_mapper.Map<ApplicationResponse>(application));
        }
    }
}