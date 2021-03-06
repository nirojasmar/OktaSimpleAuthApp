using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OktaSimpleAuth.Models;
using OktaSimpleAuth.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OktaSimpleAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserService _userService;

        public HomeController(ILogger<HomeController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize/*(Roles = "Admin")*/]
        public async Task<IActionResult> Security()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");
            return View();
        }

        [HttpGet("denied")]
        public IActionResult Denied()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet("login/{provider}")]
        public async Task LoginExternal([FromRoute] string provider,[FromQuery] string returnUrl)
        {
            if(User != null && User.Identities.Any(identity => identity.IsAuthenticated))
            {
                RedirectToAction("", "Home");
            }
            returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            var authenticationProperties = new AuthenticationProperties { RedirectUri = returnUrl };
            await HttpContext.ChallengeAsync(provider, authenticationProperties).ConfigureAwait(false);
        }

        [Route("validate")]
        public async Task<IActionResult> Validate(string User, string Password, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if(_userService.TryValidateUser(User, Password, out List<Claim> claims))
            {
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var items = new Dictionary<string, string>();
                items.Add(".AuthScheme", CookieAuthenticationDefaults.AuthenticationScheme);
                var properties = new AuthenticationProperties(items);
                await HttpContext.SignInAsync(claimsPrincipal, properties);
                return Redirect(returnUrl);
            }
            else
            {
                TempData["Error"] = "Error, User or Password is invalid";
                return View("login");
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
