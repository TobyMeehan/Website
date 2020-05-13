using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Scoreboard
    {
        public string AppId { get; set; }
        public List<Objective> Objectives { get; set; }
        public List<Score> Scores { get; set; }
    }
}
