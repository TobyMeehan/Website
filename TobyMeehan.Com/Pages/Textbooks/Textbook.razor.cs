using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Pages.Textbooks
{
    public partial class Textbook : ComponentBase
    {
        [Parameter] public string Id { get; set; }
    }
}
