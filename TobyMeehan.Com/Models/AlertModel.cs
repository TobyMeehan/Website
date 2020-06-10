using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class AlertModel
    {
        public BootstrapContext Context { get; set; }

        public RenderFragment ChildContent { get; set; }
    }
}
