using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Sender { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}
