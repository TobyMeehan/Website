using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Components
{
    public class AttributableComponentBase : ComponentBase
    {
        private Dictionary<string, object> _additionalAttributes = new Dictionary<string, object>();

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes
        {
            get => _additionalAttributes;
            
            set
            {
                if (value.TryGetValue("class", out object cssClass))
                {
                    CssClass = cssClass.ToString();
                }

                _additionalAttributes = value.Where(x => x.Key != "class").ToDictionary(x => x.Key, x => x.Value);
            }
        }

        protected string CssClass { get; set; }
    }
}
