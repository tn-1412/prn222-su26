using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceMS.Entities.NguyenDNT.Models;
using RaceMS.WebMVCApp.NguyenDNT.Models;
using RaceMS_Services.NguyenDNT;

namespace RaceMS.WebMVCApp.NguyenDNT.Controllers
{
    [Authorize]
    public class RegistrationNguyenDntsController : Controller
    {
        private readonly IRegistrationNguyenDntService _registrationService;
        private readonly IJockeyNguyenDntService _jockeyService;

        public RegistrationNguyenDntsController(
            IRegistrationNguyenDntService registrationService,
            IJockeyNguyenDntService jockeyService)
        {
            _registrationService = registrationService;
            _jockeyService = jockeyService;
        }

        // GET: RegistrationNguyenDnts?code=..&prizeMoney=..&horseName=..
        public async Task<IActionResult> Index(int? code, decimal? prizeMoney, string? horseName)
        {
            ViewData["Code"] = code;
            ViewData["PrizeMoney"] = prizeMoney;
            ViewData["HorseName"] = horseName;
            var registrations = await _registrationService.SearchAsync(code, prizeMoney, horseName);
            return View(registrations);
        }

        // GET: RegistrationNguyenDnts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var registration = await _registrationService.GetByIdAsync(id);
            if (registration == null) return NotFound();
            return View(registration);
        }

        // GET: RegistrationNguyenDnts/Create
        [Authorize(Roles = RoleNames.CanWrite)]
        public async Task<IActionResult> Create()
        {
            await PopulateJockeysAsync();
            return View();
        }

        // POST: RegistrationNguyenDnts/Create
        [HttpPost]
        [Authorize(Roles = RoleNames.CanWrite)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("RaceName,HorseName,RegisteredDate,ResponseDate,PrizeMoney,IsConfirmed,Note,JockeyNguyenDntid")]
            RegistrationNguyenDnt registration)
        {
            if (!ModelState.IsValid)
            {
                await PopulateJockeysAsync(registration.JockeyNguyenDntid);
                return View(registration);
            }

            try
            {
                await _registrationService.CreateAsync(registration);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                AddServiceError(ex);
                await PopulateJockeysAsync(registration.JockeyNguyenDntid);
                return View(registration);
            }
        }

        // GET: RegistrationNguyenDnts/Edit/5
        [Authorize(Roles = RoleNames.CanWrite)]
        public async Task<IActionResult> Edit(int id)
        {
            var registration = await _registrationService.GetByIdAsync(id);
            if (registration == null) return NotFound();
            await PopulateJockeysAsync(registration.JockeyNguyenDntid);
            return View(registration);
        }

        // POST: RegistrationNguyenDnts/Edit/5
        [HttpPost]
        [Authorize(Roles = RoleNames.CanWrite)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("RegistrationNguyenDntid,RaceName,HorseName,RegisteredDate,ResponseDate,PrizeMoney,IsConfirmed,Note,JockeyNguyenDntid")]
            RegistrationNguyenDnt registration)
        {
            if (id != registration.RegistrationNguyenDntid) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateJockeysAsync(registration.JockeyNguyenDntid);
                return View(registration);
            }

            try
            {
                await _registrationService.UpdateAsync(registration);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                AddServiceError(ex);
                await PopulateJockeysAsync(registration.JockeyNguyenDntid);
                return View(registration);
            }
        }

        // GET: RegistrationNguyenDnts/Delete/5
        [Authorize(Roles = RoleNames.CanDelete)]
        public async Task<IActionResult> Delete(int id)
        {
            var registration = await _registrationService.GetByIdAsync(id);
            if (registration == null) return NotFound();
            return View(registration);
        }

        // POST: RegistrationNguyenDnts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = RoleNames.CanDelete)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _registrationService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message.Contains(':') ? ex.Message.Split(':', 2)[1] : ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopulateJockeysAsync(int? selectedId = null)
        {
            var jockeys = await _jockeyService.GetAllAsync();
            ViewBag.Jockeys = new SelectList(jockeys, "JockeyNguyenDntid", "FullName", selectedId);
        }

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
