using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : AuthenticatedControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserProcessor _userProcessor;
        private readonly IApplicationProcessor _applicationProcessor;
        private readonly IConnectionProcessor _connectionProcessor;

        public AccountController(IMapper mapper, IUserProcessor userProcessor, IApplicationProcessor applicationProcessor, IConnectionProcessor connectionProcessor) : base (userProcessor, mapper)
        {
            _mapper = mapper;
            _userProcessor = userProcessor;
            _applicationProcessor = applicationProcessor;
            _connectionProcessor = connectionProcessor;
        }

        [HttpGet]
        public async Task<ActionResult<User>> Get()
        {
            User user = _mapper.Map<User>(await _userProcessor.GetUserById(UserId));

            return user;
        }
    }
}