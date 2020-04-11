using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace AuthWithIdentity.Controllers
{
    public class HomeController: Controller
    {
        private readonly UserManager<IdentityUser> _UserManager;

        public readonly SignInManager<IdentityUser> _SigninManager;
        private readonly IEmailService _EmailService;

        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailService emailService)
        {
            _UserManager = userManager;
            _SigninManager = signInManager;
            _EmailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            var user = await _UserManager.FindByNameAsync(username);
            if(user != null)
            {
                var res = await _SigninManager.PasswordSignInAsync(user, password, false, false);
                if (res.Succeeded)
                {
                    return RedirectToAction("Secret");
                }
            }
            return RedirectToAction("index");
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser() { UserName = username };
            var res = await _UserManager.CreateAsync(user, password);
            if (res.Succeeded)
            {
                //var res2 = await _SigninManager.PasswordSignInAsync(username, password, false, false);
                //if (res2.Succeeded)
                //{
                //    return RedirectToAction("Secret");
                //}

                //generate token and send email
                var code = await _UserManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(VerifyEmail), "Home", new { userid = user.Id, code = code }, Request.Scheme, Request.Host.ToString());
                await _EmailService.SendAsync("test@gmail.com", "Email confirmation code", $"<a href=\"{link}\">Verify email</a>");

                return RedirectToAction(nameof(EmailVerification));
            }
            return RedirectToAction("index");
        }

        public async Task<IActionResult> VerifyEmail(string userid, string code)
        {
            //check code and confirm email
            var user = await _UserManager.FindByIdAsync(userid);
            if (user == null)
                return Content("no user found");
            var res = await _UserManager.ConfirmEmailAsync(user, code);
            if (res.Succeeded)
                return View();
            return Content("unable to verify the code");
        }
        public IActionResult Authorize()
        {

            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name , "Bob"),
                new Claim(ClaimTypes.Email ,"t@gmail.com"),
                new Claim("Grandma.Says", "very nice boy")
            };
            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name , "Bon Muller"),
                new Claim(ClaimTypes.Email, "t@gmail.com"),
                new Claim("License", "A+")

            };
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity});
            HttpContext.SignInAsync(userPrincipal);
            
            return Content("You are now logged in");
        }

        public async Task<IActionResult> Logout()
        {
            await _SigninManager.SignOutAsync();
            return RedirectToAction("login");
        }


        public IActionResult EmailVerification() => View();
    }
}
