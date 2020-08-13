using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Api.Authorization;
using TobyMeehan.Com.Api.Extensions;
using TobyMeehan.Com.Api.Models;
using TobyMeehan.Com.Api.Models.Api;
using TobyMeehan.Com.AspNetCore.Extensions;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Api.Controllers.Api
{
    [Route("api/users")]
    [ApiController]
    public class UserController : OAuthControllerBase
    {
        private readonly IUserRepository _users;
        private readonly IMapper _mapper;

        public UserController(IUserRepository users, IMapper mapper)
        {
            _users = users;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var users = await _users.GetAsync();

            return Ok(_mapper.Map<List<PartialUserResponse>>(users));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            if (id == "@me")
            {
                id = UserId;
            }

            var user = _mapper.Map<UserModel>(await _users.GetByIdAsync(id));

            if (user == null)
            {
                return NotFound(new ErrorResponse("User does not exist."));
            }

            if (user.Id == User.Id() && User.HasScope(Scopes.Identify))
            {
                return Ok(_mapper.Map<UserResponse>(user));
            }
            else
            {
                return Ok(_mapper.Map<PartialUserResponse>(user));
            }
        }
    }
}