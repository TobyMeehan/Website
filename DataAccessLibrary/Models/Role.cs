using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Role : EntityBase
    {
        public string Name { get; set; }
    }

    public class UserRole
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }

    }
}
