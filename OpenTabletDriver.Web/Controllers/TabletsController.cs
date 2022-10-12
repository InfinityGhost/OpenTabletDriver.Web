using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenTabletDriver.Web.Core.Services;
using OpenTabletDriver.Web.Models;

namespace OpenTabletDriver.Web.Controllers
{
    public class TabletsController : Controller
    {
        private ITabletService _tabletService;

        public TabletsController(ITabletService tabletService)
        {
            _tabletService = tabletService;
        }

        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> Index(string search = null)
        {
            var configs = from config in await _tabletService.GetConfigurations()
                orderby config.Metadata.SupportStatus descending, config.ToString()
                select config;

            var model = new TabletsViewModel
            {
                Search = search,
                Configurations = configs.ToList()
            };

            return View(model);
        }
    }
}
