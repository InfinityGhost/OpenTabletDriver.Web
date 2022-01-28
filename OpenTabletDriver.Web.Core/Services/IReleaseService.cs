using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using OpenTabletDriver.Web.Core.Contracts;

namespace OpenTabletDriver.Web.Core.Services
{
    public interface IReleaseService
    {
        Task<IEnumerable<IRelease>> GetAllReleases();

        Task<IRelease> GetLatestRelease();

        Task<IRelease> GetRelease(string tag);
        Task<Repository> GetRepository();
    }
}