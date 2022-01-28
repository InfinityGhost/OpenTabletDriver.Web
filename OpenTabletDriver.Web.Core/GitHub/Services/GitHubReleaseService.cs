using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using OpenTabletDriver.Web.Core.Contracts;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Core.GitHub.Services
{
    public class GitHubReleaseService : IReleaseService
    {
        private IServiceProvider serviceProvider;
        private IGitHubClient client;

        public GitHubReleaseService(IServiceProvider serviceProvider, IGitHubClient client)
        {
            this.serviceProvider = serviceProvider;
            this.client = client;

            GetRepositoryCached = new CachedTask<Repository>(GetRepositoryInternal(), CacheTime);
            GetLatestReleaseCached = new CachedTask<IRelease>(GetLatestReleaseInternal(), CacheTime);
            GetAllReleasesCached = new CachedTask<IEnumerable<IRelease>>(GetAllReleasesInternal(), CacheTime);
        }

        private const string REPOSITORY_OWNER = "OpenTabletDriver";
        private const string REPOSITORY_NAME = "OpenTabletDriver";

        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(1);

        private CachedTask<Repository> GetRepositoryCached { get; }
        private CachedTask<IEnumerable<IRelease>> GetAllReleasesCached { get; }
        private CachedTask<IRelease> GetLatestReleaseCached { get; }

        public Task<Repository> GetRepository()
        {
            return GetRepositoryCached;
        }

        public Task<IEnumerable<IRelease>> GetAllReleases()
        {
            return GetAllReleasesCached;
        }

        public Task<IRelease> GetLatestRelease()
        {
            return GetLatestReleaseCached;
        }

        public async Task<IRelease> GetRelease(string tag)
        {
            var repo = await GetRepositoryCached.Get();
            var release = await client.Repository.Release.Get(repo.Id, tag);

            return serviceProvider.CreateInstance<GitHubRelease>(release);
        }

        private async Task<IRelease> GetLatestReleaseInternal()
        {
            var repo = await GetRepositoryCached.Get();
            var release = await client.Repository.Release.GetLatest(repo.Id);

            return serviceProvider.CreateInstance<GitHubRelease>(release);
        }

        private async Task<IEnumerable<IRelease>> GetAllReleasesInternal()
        {
            var repo = await GetRepositoryCached.Get();
            var releases = await client.Repository.Release.GetAll(repo.Id);
            return releases.Select(r => serviceProvider.CreateInstance<GitHubRelease>(r));
        }

        private Task<Repository> GetRepositoryInternal()
        {
            return client.Repository.Get(REPOSITORY_OWNER, REPOSITORY_NAME);
        }
    }
}