using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthWithPolicy.Controllers
{
    public class HomeController : Controller
    {

        // GET: /<controller>/
        public IActionResult Index()
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
            var ucName = new Claim(ClaimTypes.Name, username);
            var ucEmail = new Claim(ClaimTypes.Email, username);
            var ucPass = new Claim("Password", password);
            var ucClaim = new Claim(ClaimTypes.DateOfBirth, "01/01/2011");
            var ucrole = new Claim(ClaimTypes.Role, "Admin");
            var ci = new ClaimsIdentity(new List<Claim>() { ucName, ucEmail, ucPass, ucClaim, ucrole},"gmclaimsIdentity");
            var cp = new ClaimsPrincipal(new[] { ci });
            await HttpContext.SignInAsync(cp);
            return RedirectToAction(nameof(Secret));
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(Policy = "Claim.Dob")]
        public IActionResult ClaimSecret()
        {
            return View(nameof(Secret));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RoleSecret()
        {
            return View(nameof(Secret));
        }



    }
}
