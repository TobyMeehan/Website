using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Score
    {
        public string Id { get; set; }
        public Objective Objective { get; set; }
        public SimplifiedUser User { get; set; }
        public int Value { get; set; }
    }
}
