using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Pages.Downloads
{
    public partial class DownloadLink : ComponentBase
    {
        [Parameter] public string Id { get; set; }

        [Parameter] public ActionType Action { get; set; }

        [Parameter] public bool NavLink { get; set; } = false;

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> InputAttributes { get; set; }

        private string _action;

        public enum ActionType
        {
            View,
            Edit,
            Delete,
            Add,
            Files,
            Authors
        }

        protected override void OnInitialized()
        {
            _action = "/downloads";
            switch (Action)
            {
                case ActionType.View:
                    _action += $"/{Id}";
                    break;
                case ActionType.Edit:
                    _action += $"/{Id}/edit";
                    break;
                case ActionType.Delete:
                    _action += $"/{Id}/delete";
                    break;
                case ActionType.Add:
                    _action += "/add";
                    break;
                case ActionType.Files:
                    _action += $"/{Id}/files";
                    break;
                case ActionType.Authors:
                    _action += $"/{Id}/authors";
                    break;
                default:
                    _action += "";
                    break;
            }
        }
    }
}
