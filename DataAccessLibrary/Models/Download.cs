using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Download
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public List<string> Files { get; set; }
        public string Version { get; set; }
        public DateTime Updated { get; set; }
        public List<User> Authors { get; set; }

    }
}
