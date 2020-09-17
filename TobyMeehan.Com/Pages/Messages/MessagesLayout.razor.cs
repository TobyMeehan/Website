using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.AspNetCore.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Messages
{
    public partial class MessagesLayout : LayoutComponentBase
    {
        [Inject] private IConversationRepository conversations { get; set; }
        [Inject] private NavigationManager navigation { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private ConversationViewModel _conversationForm = new ConversationViewModel();

        private IList<Conversation> _conversations;
        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _context = await AuthenticationStateTask;
            _conversations = await Task.Run(async () => await conversations.GetByUserAsync(_context.User.Id()));
        }

        private async Task AddForm_Submit()
        {
            var conversation = await conversations.AddAsync(_conversationForm.Name, _context.User.Id());

            navigation.NavigateTo($"/messages/conversations/{conversation.Id}");
        }
    }
}
