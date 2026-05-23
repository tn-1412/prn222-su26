using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RaceMS.Entities.NguyenDNT.Models;
using RaceMS_Repositories.NguyenDNT.DBContext;
using RaceMS_Services.NguyenDNT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: JockeyNguyenDnt
        public async Task<IActionResult> Index()
        {
            var jockeys = await jockeyNguyenDntService.GetAllAsync();
            return View(jockeys);
        }
    }
}
