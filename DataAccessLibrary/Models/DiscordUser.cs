using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class DiscordUser
    {
        public ulong Id { get; set; }

        public string Username { get; set; }

        public int Discriminator { get; set; }

        public string AvatarUrl { get; set; }
    }
}
