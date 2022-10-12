using System.Collections.Generic;
using OpenTabletDriver.Tablet;

namespace OpenTabletDriver.Web.Models
{
    public class TabletsViewModel
    {
        public string Search { set; get; }
        public IList<TabletConfiguration> Configurations { set; get; }
    }
}
