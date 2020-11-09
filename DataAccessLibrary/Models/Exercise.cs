using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Exercise : EntityBase
    {
        public string ChapterId { get; set; }
        public string Title { get; set; }
        public string SolutionBankUrl { get; set; }
    }
}
