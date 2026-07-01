using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceMS.Entities.NguyenDNT.Models;
using RaceMS.WebMVCApp.NguyenDNT.Models;
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
                    // RoleId chưa map được (không nằm trong enum UserRole) → không gán role name,
                    // tài khoản vẫn đăng nhập được nhưng không thuộc role nào → chỉ có quyền xem.
                    var roleName = Enum.IsDefined(typeof(UserRole), userAccount.RoleId)
                        ? ((UserRole)userAccount.RoleId).ToString()
                        : null;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userAccount.UserName),
                        new Claim(ClaimTypes.GivenName, userAccount.FullName ?? userAccount.UserName)
                    };
                    if (roleName != null)
                        claims.Add(new Claim(ClaimTypes.Role, roleName));

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

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "JockeyNguyenDnts");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(model.ConfirmPassword), "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            var account = new SystemUserAccount
            {
                UserName = model.UserName?.Trim(),
                Password = model.Password,
                FullName = model.FullName?.Trim(),
                Email = model.Email?.Trim(),
                Phone = model.Phone?.Trim(),
                EmployeeCode = model.EmployeeCode?.Trim()
            };

            try
            {
                await _userAccountService.RegisterAsync(account);
                TempData["RegisterSuccess"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (InvalidOperationException ex)
            {
                AddServiceError(ex);
                return View(model);
            }
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

        // Parse "FieldName:Thông báo" → ModelState.AddModelError(field, message)
        private void AddServiceError(InvalidOperationException ex)
        {
            var parts = ex.Message.Split(':', 2);
            if (parts.Length == 2)
                ModelState.AddModelError(parts[0], parts[1]);
            else
                ModelState.AddModelError(string.Empty, ex.Message);
        }
    }
}
