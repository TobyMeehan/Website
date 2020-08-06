using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.Api
{
    public class ScoreResponse
    {
        public PartialUserResponse User { get; set; }
        public int Value { get; set; }
    }
}
