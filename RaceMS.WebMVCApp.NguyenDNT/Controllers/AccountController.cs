using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceMS_Services.NguyenDNT;
using System.Security.Claims;

namespace RaceMS.WebMVCApp.NguyenDNT.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISystemUserAccountService _userAccountService;

        public AccountController(ISystemUserAccountService userAccountService) => _userAccountService = userAccountService;

        [AllowAnonymous]
        public IActionResult Index() => RedirectToAction("Login");

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "JockeyNguyenDnts");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password)
        {
            try
            {
                var userAccount = await _userAccountService.GetUserAccountAsync(userName, password);

                if (userAccount != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userAccount.UserName),
                        new Claim(ClaimTypes.GivenName, userAccount.FullName ?? userAccount.UserName),
                        new Claim(ClaimTypes.Role, userAccount.RoleId.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties { IsPersistent = false });

                    return RedirectToAction("Index", "JockeyNguyenDnts");
                }
            }
            catch (Exception)
            {
            }

            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("Role");

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public IActionResult Forbidden() => View();
    }
}
