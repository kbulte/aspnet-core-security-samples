using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using System.Collections.Generic;

namespace CookieAuthenticationMiddleware.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!(string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(password)))
            {
                var claims = new List<Claim>()
                {
                    new Claim("name", "kris"),
                    new Claim("age", "29")
                };

                ////As claims can be attached to an unauthenticated user, in order to get an authenticated user we should set the AuthenticationType parameter
                var identity = new ClaimsIdentity(claims, "My asp net core application");

                ////The HttpContext gives access to the authenticationManager
                ////Since we can have multiple authentication schemes we have to provide the name of the authentication scheme responsible to sign the user in
                await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(identity));

                //When logged in you want to return to the returnUrl but be sure this url is a local url, the following does the check for you
                return new LocalRedirectResult(returnUrl);
            }

            return View();
        }
    }
}