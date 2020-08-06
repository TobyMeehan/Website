using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.Api
{
    public class ObjectiveResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ScoreResponse> Scores { get; set; }
    }
}
