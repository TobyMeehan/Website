using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class AlertModel
    {
        public int AlertId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Context { get; set; }

    }
}
