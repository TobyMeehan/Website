using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.Api
{
    public class TransactionResponse
    {
        public string Id { get; set; }
        public string AppId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}
