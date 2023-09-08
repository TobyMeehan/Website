using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TobyMeehan.Com.Accounts.Extensions;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IApplicationService _applications;

    public IndexModel(ILogger<IndexModel> logger, IApplicationService applications)
    {
        _logger = logger;
        _applications = applications;
    }

    public IEntityCollection<IApplication> Applications { get; set; }

    [BindProperty] public ApplicationFormModel ApplicationForm { get; set; } = new();
    
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (!(User.Identity?.IsAuthenticated ?? false)) return Redirect("/login");
        
        Applications = await _applications.GetByAuthorAsync(User.Id(), cancellationToken);
            
        return Page();

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var application = await _applications.CreateAsync(new CreateApplicationBuilder()
            .WithName(ApplicationForm.Name)
            .WithAuthor(User.Id()));

        await _applications.AddRedirectAsync(application.Id, new Uri(ApplicationForm.RedirectUri));

        ApplicationForm = new();

        Applications = await _applications.GetByAuthorAsync(User.Id());

        return Page();
    }
}