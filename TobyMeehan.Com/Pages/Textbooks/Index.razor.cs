using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Textbooks
{
    public partial class Index : ComponentBase
    {
        protected override void OnInitialized()
        {
            _textbooks = GetTextbooks();

            base.OnInitialized();
        }

        private List<TextbookViewModel> _textbooks;

        private List<TextbookViewModel> GetTextbooks()
        {
            return new List<TextbookViewModel>
            {
                new TextbookViewModel("pure1", "Pure Year 1"),
                new TextbookViewModel("pure2", "Pure Year 2"),
                new TextbookViewModel("applied1", "Applied Year 1"),
                new TextbookViewModel("applied2", "Applied Year 2"),
                new TextbookViewModel("core1", "Further Core Pure Book 1"),
                new TextbookViewModel("core2", "Further Core Pure Book 2"),
                new TextbookViewModel("decision1", "Decision Mathematics 1"),
                new TextbookViewModel("fm1", "Futher Mechanics 1")
            };
        }          
    }
}
