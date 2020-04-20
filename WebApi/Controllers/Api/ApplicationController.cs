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

namespace WebApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApplicationController : AuthenticatedControllerBase
    {
        private readonly IApplicationProcessor _applicationProcessor;
        private readonly IMapper _mapper;

        public ApplicationController(IApplicationProcessor applicationProcessor, IUserProcessor userProcessor, IMapper mapper) : base(userProcessor, mapper)
        {
            _applicationProcessor = applicationProcessor;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(AppId));

            return Ok(new
            {
                app.Id,
                app.Name
            });
        }
    }
}