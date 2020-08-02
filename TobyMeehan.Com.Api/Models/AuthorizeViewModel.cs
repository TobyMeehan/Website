using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models
{
    public class AuthorizeViewModel
    {
        public ConnectionModel Connection { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }
}
