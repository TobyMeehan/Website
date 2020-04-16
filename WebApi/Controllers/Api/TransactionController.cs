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
    public class TransactionController : AuthenticatedControllerBase
    {
        private readonly IUserProcessor _userProcessor;
        private readonly IApplicationProcessor _applicationProcessor;
        private readonly IMapper _mapper;

        public TransactionController(IUserProcessor userProcessor, IApplicationProcessor applicationProcessor, IMapper mapper) : base (userProcessor, mapper)
        {
            _userProcessor = userProcessor;
            _applicationProcessor = applicationProcessor;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Transaction transaction)
        {
            transaction.UserId = UserId;
            transaction.Sender = (await _applicationProcessor.GetApplicationById(AppId)).Name;

            if (await _userProcessor.TrySendTransaction(_mapper.Map<DataAccessLibrary.Models.Transaction>(transaction)))
            {
                return Ok();
            }
            else
            {
                return StatusCode(402);
            }
        }
    }
}