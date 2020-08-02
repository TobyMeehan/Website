using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.Api
{
    public class PartialUserResponse
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public List<RoleResponse> Roles { get; set; }
    }
}
