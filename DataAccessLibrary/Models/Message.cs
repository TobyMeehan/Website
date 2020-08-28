﻿using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("messages")]
    public class Message : MessageBase
    {
        public string ConversationId { get; set; }
    }
}
