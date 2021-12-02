using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class DiscordToken : WebToken
    {
        public string Id { get; set; }
        public ulong DiscordId { get; set; }
        public DateTime Created { get; set; }
    }
}
