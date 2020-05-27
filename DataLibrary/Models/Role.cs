using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("roles")]
    public class Role : IEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    [SqlName("userroles")]
    public class UserRole
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }

    }
}
