using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

/// <summary>
/// PageModel responsible for rendering the Privacy page of the application.
/// </summary>
public class PrivacyModel : PageModel
{
    /// <summary>
    /// Logger used for diagnostic and application logging.
    /// </summary>
    private readonly ILogger<PrivacyModel> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrivacyModel"/> class.
    /// </summary>
    /// <param name="logger">
    /// Logger instance provided through dependency injection.
    /// </param>
    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles HTTP GET requests for the Privacy page.
    /// </summary>
    /// <remarks>
    /// This method does not perform any operations, as the Privacy page
    /// consists solely of static content rendered by the Razor view.
    /// </remarks>
    public void OnGet()
    {
    }
}