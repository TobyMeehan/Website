using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Collections;

namespace TobyMeehan.Com.Data.Models
{
    public class Objective : EntityBase
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public IList<Score> Scores { get; set; }
    }
}
