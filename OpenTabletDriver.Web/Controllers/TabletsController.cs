using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTabletDriver.Web.Core.Services;
using OpenTabletDriver.Web.Models;

namespace OpenTabletDriver.Web.Controllers
{
    public class TabletsController : Controller
    {
        private ITabletService tabletService;
        private IReleaseService releaseService;
        private ILogger<TabletsController> logger;

        public TabletsController(ITabletService tabletService, IReleaseService releaseService,
            ILogger<TabletsController> logger)
        {
            this.tabletService = tabletService;
            this.releaseService = releaseService;
            this.logger = logger;
        }

        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> Index(string search = null)
        {
            logger.Log(LogLevel.Information, "Fetching Tablets table...");

            var repoContent = await releaseService.GetRepositoryContent();
            logger.Log(LogLevel.Information, "Fetched repository content");

            var file = repoContent.First(r => r.Name == "TABLETS.md");
            logger.Log(LogLevel.Information, $"Found TABLETS.md: {file.Size} bytes at {file.DownloadUrl}");

            using (var httpClient = new HttpClient())
            {
                logger.Log(LogLevel.Information, "Downloading TABLETS.md...");
                var markdown = await httpClient.GetStringAsync(file.DownloadUrl);

                logger.Log(LogLevel.Information, "Successfully downloaded TABLETS.md");

                string html = Markdown.ToHtml(markdown);
                string patchedHtml = html.Replace(
                    "<table>",
                    "<table id=\"tablets\" class=\"table table-hover\">"
                );

                var model = new ContentSearchViewModel
                {
                    Content = new HtmlString(patchedHtml),
                    Search = search
                };
                return View(model);
            }
        }
    }
}