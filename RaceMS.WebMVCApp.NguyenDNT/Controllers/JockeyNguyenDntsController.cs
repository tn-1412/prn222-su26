using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceMS.Entities.NguyenDNT.Models;
using RaceMS_Services.NguyenDNT;

namespace RaceMS.WebMVCApp.NguyenDNT.Controllers
{
    [Authorize]
    public class JockeyNguyenDntsController : Controller
    {
        private readonly IJockeyNguyenDntService _jockeyService;

        public JockeyNguyenDntsController(IJockeyNguyenDntService jockeyService)
        {
            _jockeyService = jockeyService;
        }

        // GET: JockeyNguyenDnts
        public async Task<IActionResult> Index()
        {
            var jockeys = await _jockeyService.GetAllAsync();
            return View(jockeys);
        }

        // GET: JockeyNguyenDnts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var jockey = await _jockeyService.GetByIdAsync(id);
            if (jockey == null) return NotFound();
            return View(jockey);
        }

        // GET: JockeyNguyenDnts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JockeyNguyenDnts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FullName,PhoneNumber,Email,LicenseCode,Weight,DateOfBirth,IsActive")]
            JockeyNguyenDnt jockey)
        {
            if (!ModelState.IsValid) return View(jockey);

            try
            {
                await _jockeyService.CreateAsync(jockey);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                AddServiceError(ex);
                return View(jockey);
            }
        }

        // GET: JockeyNguyenDnts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var jockey = await _jockeyService.GetByIdAsync(id);
            if (jockey == null) return NotFound();
            return View(jockey);
        }

        // POST: JockeyNguyenDnts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("JockeyNguyenDntid,FullName,PhoneNumber,Email,LicenseCode,Weight,DateOfBirth,IsActive")]
            JockeyNguyenDnt jockey)
        {
            if (id != jockey.JockeyNguyenDntid) return BadRequest();
            if (!ModelState.IsValid) return View(jockey);

            try
            {
                await _jockeyService.UpdateAsync(jockey);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                AddServiceError(ex);
                return View(jockey);
            }
        }

        // GET: JockeyNguyenDnts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var jockey = await _jockeyService.GetByIdAsync(id);
            if (jockey == null) return NotFound();
            return View(jockey);
        }

        // POST: JockeyNguyenDnts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _jockeyService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Jockey không tồn tại — về Index với thông báo
                TempData["Error"] = ex.Message.Contains(':') ? ex.Message.Split(':', 2)[1] : ex.Message;
                return RedirectToAction(nameof(Index));
            }
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
