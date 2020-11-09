using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Chapter : EntityBase
    {
        public string TextbookId { get; set; }
        public string Title { get; set; }
        public EntityCollection<Exercise> Exercises { get; set; }
    }
}
