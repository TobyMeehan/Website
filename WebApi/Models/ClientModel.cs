using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ClientModel
    {
        public string AppID { get; set; }
        public string Name { get; set; }
        public UserModel Author { get; set; }
        public string Secret { get; set; }
        public List<RoleModel> Roles { get; set; }

    }
}
