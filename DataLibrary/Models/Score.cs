using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Score : EntityBase
    {
        public string ObjectiveId { get; set; }
        public Objective Objective { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int Value { get; set; }
    }
}
