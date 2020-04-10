using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class Connection
    {
        public string Id { get; set; }
        public Application Application { get; set; }
        public User User { get; set; }
    }
}
