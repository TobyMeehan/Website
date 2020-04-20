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
    public class TransactionController : AuthenticatedControllerBase
    {
        private readonly IUserProcessor _userProcessor;
        private readonly IApplicationProcessor _applicationProcessor;
        private readonly IMapper _mapper;

        public TransactionController(IUserProcessor userProcessor, IApplicationProcessor applicationProcessor, IMapper mapper) : base(userProcessor, mapper)
        {
            _userProcessor = userProcessor;
            _applicationProcessor = applicationProcessor;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Transaction>>> Get()
        {
            return _mapper.Map<List<Transaction>>((await _userProcessor.GetUserById(UserId)).Transactions);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Transaction transaction)
        {
            if (await _userProcessor.TrySendTransaction(new DataAccessLibrary.Models.Transaction
            {
                UserId = UserId,
                Sender = (await _applicationProcessor.GetApplicationById(AppId)).Name,
                Description = transaction.Description,
                Amount = transaction.Amount
            }))
            {
                return Ok();
            }
            else
            {
                return StatusCode(402, new ErrorResponse { Error = "Transaction failed." });
            }
        }
    }
}