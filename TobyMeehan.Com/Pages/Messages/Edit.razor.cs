using AutoMapper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Messages
{
    public partial class Edit : ComponentBase
    {
        [Inject] public IConversationRepository conversations { get; set; }
        [Inject] public IMapper mapper { get; set; }

        [Parameter] public string Id { get; set; }

        private ConversationViewModel _model = new ConversationViewModel();
        private Conversation _conversation;

        protected override async Task OnInitializedAsync()
        {
            _conversation = await Task.Run(async () => await conversations.GetByIdAsync(Id));
            _model = mapper.Map<ConversationViewModel>(_conversation);
        }
    }
}
