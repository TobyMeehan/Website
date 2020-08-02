using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.Api
{
    public class UserResponse : PartialUserResponse
    {
        public int Balance { get; set; }
    }
}
