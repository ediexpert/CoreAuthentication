using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Basic.Controllers
{
    public class HomeController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public IActionResult Secret()
        {
            return View();
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

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
