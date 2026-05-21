using Microsoft.AspNetCore.Mvc;
using RaceMS_Services.NguyenDNT;

namespace RaceMS.WebMVCApp.NguyenDNT.Controllers
{
    public class JockeyNguyenDntController : Controller
    {
        private readonly IJockeyNguyenDntService jockeyNguyenDntService;
        private readonly IRegistrationNguyenDntService registrationNguyenDntService;
        public JockeyNguyenDntController(IJockeyNguyenDntService jockeyNguyenDntService, IRegistrationNguyenDntService registrationNguyenDntService)
        {
            this.jockeyNguyenDntService = jockeyNguyenDntService;
            this.registrationNguyenDntService = registrationNguyenDntService;
        }
        public async Task<IActionResult> Index()
        {
            var jockeys = await jockeyNguyenDntService.GetAllAsync();
            return View(jockeys);

        }
    }
}

