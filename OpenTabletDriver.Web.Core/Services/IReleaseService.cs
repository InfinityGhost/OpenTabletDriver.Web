using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using OpenTabletDriver.Web.Core.Contracts;

namespace OpenTabletDriver.Web.Core.Services
{
    public interface IReleaseService
    {
        Task<Repository> GetRepository();
        Task<IReadOnlyList<RepositoryContent>> GetRepositoryContent();
        Task<IEnumerable<IRelease>> GetAllReleases();

        Task<IRelease> GetLatestRelease();

        Task<IRelease> GetRelease(string tag);
    }
}