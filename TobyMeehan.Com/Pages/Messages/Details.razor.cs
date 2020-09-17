using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Pages.Messages
{
    public partial class Details : ComponentBase
    {
        [Inject] private IConversationRepository conversations { get; set; }
        [Inject] private IMessageRepository messages { get; set; }

        [Parameter] public string Id { get; set; }

        private Conversation _conversation;
        private IEnumerable<Message> _messages;

        protected override async Task OnInitializedAsync()
        {
            _conversation = await Task.Run(async () => await conversations.GetByIdAsync(Id));
            _messages = await Task.Run(async () => await messages.GetByConversationAsync(Id));
        }
    }
}
