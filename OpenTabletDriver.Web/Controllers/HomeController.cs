using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTabletDriver.Web.Core.Services;
using OpenTabletDriver.Web.Models;

namespace OpenTabletDriver.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IReleaseService _releaseService;

        public HomeController(ILogger<HomeController> logger, IReleaseService releaseService)
        {
            _logger = logger;
            _releaseService = releaseService;
        }

        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> Index()
        {
            var release = await _releaseService.GetLatestRelease();
            return View(release);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var viewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}
