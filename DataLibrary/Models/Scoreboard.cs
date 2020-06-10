using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Scoreboard
    {
        public string AppId { get; set; }
        public EntityCollection<Objective> Objectives { get; set; } = new EntityCollection<Objective>();
        public EntityCollection<Score> Scores { get; set; } = new EntityCollection<Score>();
    }
}
