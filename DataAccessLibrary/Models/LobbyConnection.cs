using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class LobbyConnection : EntityBase
    {
        public string LobbyId { get; set; }
        public string UserId { get; set; }

        public string DisplayName { get; set; }

        public User User { get; set; }
    }
}
