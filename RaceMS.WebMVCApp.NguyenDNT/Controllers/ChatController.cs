using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RaceMS.WebMVCApp.NguyenDNT.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        public IActionResult Index() => View();
    }
}
