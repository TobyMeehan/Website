using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("users")]
    public class User : EntityBase
    {
        public string Username { get; set; }
        public string VanityUrl { get; set; }
        public string Email { get; set; }
        public int Balance { get; set; }
        public string HashedPassword { get; set; }
        public string Description { get; set; }
        public string ProfilePictureUrl { get; set; }
        public IEntityCollection<Role> Roles { get; set; } = new EntityCollection<Role>();
    }
}
