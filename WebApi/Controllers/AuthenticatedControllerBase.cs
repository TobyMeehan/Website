using AutoMapper;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class AuthenticatedControllerBase : ControllerBase
    {
        private readonly IUserProcessor _userProcessor;
        private readonly IMapper _mapper;

        public AuthenticatedControllerBase(IUserProcessor userProcessor, IMapper mapper)
        {
            _userProcessor = userProcessor;
            _mapper = mapper;
        }

        public string UserId => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        public string Username => User.Identity.Name;

        public string AppId => User.Claims.First(c => c.Type == ClaimTypes.Actor).Value;

        public async Task<User> GetUser() => _mapper.Map<User>(await _userProcessor.GetUserById(UserId));
    }
}
