using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Textbook : EntityBase
    {
        public string Title { get; set; }
        public EntityCollection<Chapter> Chapters { get; set; }
    }
}
