using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class ButtonPress : EntityBase
    {
        public string UserId { get; set; }
        public DateTime TimePressed { get; set; }
        public int ButtonSeconds { get; set; }
        public TimeSpan ButtonTimeSpan => TimeSpan.FromSeconds(ButtonSeconds);
    }
}
