using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    public class AccountController : Controller
    {
        // Redirects the user to the Auth0 login page.
        // After login, Auth0 redirects back to the returnUrl (defaults to the home page).
        public async Task Login(string returnUrl = "/")
        {
            // Build the properties that tell Auth0 where to send the user after login
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            // Challenge kicks off the OpenID Connect flow, sending the user to Auth0
            await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        }

        // Logs the user out of both the app and Auth0.
        // [Authorize] ensures anonymous users cannot hit this endpoint directly.
        [Authorize]
        public async Task Logout()
        {
            // Build the properties that tell Auth0 where to send the user after logout
            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri(Url.Action("Index", "Home")!)
                .Build();

            // Sign out of Auth0 (clears the Auth0 session)
            await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);

            // Sign out of the local cookie session
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
