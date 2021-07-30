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
    [Route("api/users/{id}/transactions")]
    [ApiController]
    public class TransactionController : OAuthControllerBase
    {
        private readonly IUserRepository _users;
        private readonly ITransactionRepository _transactions;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public TransactionController(IUserRepository users, ITransactionRepository transactions, IMapper mapper, IAuthorizationService authorizationService)
        {
            _users = users;
            _transactions = transactions;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(ScopePolicies.HasTransactionsScope)]
        public async Task<IActionResult> Get(string id)
        {
            if (id == "@me")
            {
                id = UserId;
            }

            if (id != UserId)
            {
                return Forbid(new ErrorResponse("The transactions scope is required to get a user's transactions."));
            }

            var transactions = await _transactions.GetByUserAsync(id);

            return Ok(_mapper.Map<List<TransactionResponse>>(transactions));
        }

        [HttpPost]
        [Authorize(ScopePolicies.HasTransactionsScope)]
        public async Task<IActionResult> Post(string id, [FromBody] TransactionRequest request, [FromQuery] bool? allowNegative)
        {
            if (id == "@me")
            {
                id = UserId;
            }

            if (id != UserId)
            {
                return Forbid(new ErrorResponse("The transactions scope is required to send a user transactions."));
            }

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