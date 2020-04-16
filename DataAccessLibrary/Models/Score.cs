using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Score
    {
        public string Id { get; set; }
        public string ObjectiveId { get; set; }
        public Objective Objective { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int Value { get; set; }

    }
}
