using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Controllers
{
    public class ChangelogController : Controller
    {
        public ChangelogController(IReleaseService releaseService)
        {
            _releaseService = releaseService;
        }

        private IReleaseService _releaseService;

        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> Index()
        {
            var releases = await _releaseService.GetAllReleases();
            return View(releases);
        }

        public async Task<IActionResult> GetChangelog([NotNull] string tag)
        {
            var release = await _releaseService.GetRelease(tag);
            return PartialView("Release/_Changelog", release);
        }
    }
}
