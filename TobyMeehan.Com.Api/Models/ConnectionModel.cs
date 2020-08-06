using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models
{
    public class ConnectionModel : EntityModel
    {
        public UserModel User { get; set; }
        public ApplicationModel Application { get; set; }
    }
}
