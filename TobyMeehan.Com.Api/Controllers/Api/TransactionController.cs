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
    [Route("api/users/@me/transactions")]
    [ApiController]
    public class TransactionController : OAuthControllerBase
    {
        private readonly IUserRepository _users;
        private readonly IMapper _mapper;

        public TransactionController(IUserRepository users, IMapper mapper)
        {
            _users = users;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Scope("transactions")]
        public async Task<IActionResult> Get()
        {
            var user = await _users.GetByIdAsync(UserId);

            return Ok(_mapper.Map<List<TransactionResponse>>(user.Transactions));
        }

        [HttpPost]
        [Authorize]
        [Scope("transactions")]
        public async Task<IActionResult> Post(TransactionRequest request, [FromQuery] bool? allowNegative)
        {
            var user = await _users.GetByIdAsync(UserId);

            if (user.Balance + request.Amount < 0 && !(allowNegative ?? false))
            {
                return BadRequest(new ErrorResponse("Resulting balance is negative and client specified allowNegative parameter as false."));
            }

            var transaction = await _users.AddTransactionAsync(UserId, AppId, request.Description, request.Amount);

            return Ok(_mapper.Map<TransactionResponse>(transaction));
        }
    }
}