using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Alert
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Context { get; set; }

    }
}
