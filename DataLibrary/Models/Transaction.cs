using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Transaction : EntityBase
    {
        public string Sender { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}
