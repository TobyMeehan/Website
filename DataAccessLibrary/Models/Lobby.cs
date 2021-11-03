using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Lobby : EntityBase
    {
        public string Name { get; set; }

        public IList<LobbyConnection> Connections { get; set; }
    }
}
