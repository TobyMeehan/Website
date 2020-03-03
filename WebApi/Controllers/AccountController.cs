using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserProcessor _userProcessor;
        private readonly IApplicationProcessor _applicationProcessor;
        private readonly IConnectionProcessor _connectionProcessor;

        public AccountController(IMapper mapper, IUserProcessor userProcessor, IApplicationProcessor applicationProcessor, IConnectionProcessor connectionProcessor)
        {
            _mapper = mapper;
            _userProcessor = userProcessor;
            _applicationProcessor = applicationProcessor;
            _connectionProcessor = connectionProcessor;
        }

        public async Task<User> Get()
        {
            return _mapper.Map<User>(await _userProcessor.GetUserById(User.Identity.Name));
        }
    }
}