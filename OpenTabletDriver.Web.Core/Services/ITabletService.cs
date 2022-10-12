using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTabletDriver.Tablet;

namespace OpenTabletDriver.Web.Core.Services
{
    public interface ITabletService
    {
        Task<IList<TabletConfiguration>> GetConfigurations();
    }
}
