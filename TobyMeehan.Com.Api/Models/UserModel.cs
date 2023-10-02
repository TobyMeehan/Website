using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models
{
    public class UserModel : EntityModel
    {
        public string Username { get; set; }
        public int Balance { get; set; }
        public string ProfilePictureUrl { get; set; }
        public IList<RoleModel> Roles { get; set; }
    }
}
