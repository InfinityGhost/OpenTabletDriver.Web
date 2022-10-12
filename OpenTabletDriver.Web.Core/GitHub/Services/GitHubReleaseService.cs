using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Octokit;
using OpenTabletDriver.Web.Core.Contracts;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Core.GitHub.Services
{
    public class GitHubReleaseService : IReleaseService
    {
        private IServiceProvider _serviceProvider;
        private IRepositoryService _repositoryService;
        private IGitHubClient _client;
        private IMemoryCache _cache;

        public GitHubReleaseService(
            IServiceProvider serviceProvider,
            IRepositoryService repositoryService,
            IGitHubClient client,
            IMemoryCache cache
        )
        {
            _serviceProvider = serviceProvider;
            _repositoryService = repositoryService;
            _client = client;
            _cache = cache;
        }

        private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(10);

        public Task<IReadOnlyList<IRelease>> GetAllReleases()
        {
            const string key = nameof(GitHubReleaseService) + nameof(GetAllReleases);
            return _cache.GetOrCreateAsync(key, GetAllReleasesInternal);
        }

        public Task<IRelease> GetLatestRelease()
        {
            const string key = nameof(GitHubReleaseService) + nameof(GetLatestRelease);
            return _cache.GetOrCreateAsync(key, GetLatestReleaseInternal);
        }

        public async Task<IRelease> GetRelease(string tag)
        {
            var repo = await _repositoryService.GetRepository();
            var release = await _client.Repository.Release.Get(repo.Id, tag);

            return _serviceProvider.CreateInstance<GitHubRelease>(release);
        }

        private async Task<IRelease> GetLatestReleaseInternal(ICacheEntry entry)
        {
            entry.AbsoluteExpirationRelativeToNow = Expiration;

            var repo = await _repositoryService.GetRepository();
            var release = await _client.Repository.Release.GetLatest(repo.Id);

            return _serviceProvider.CreateInstance<GitHubRelease>(release);
        }

        private async Task<IReadOnlyList<IRelease>> GetAllReleasesInternal(ICacheEntry entry)
        {
            entry.AbsoluteExpirationRelativeToNow = Expiration;

            var repo = await _repositoryService.GetRepository();
            var releases = await _client.Repository.Release.GetAll(repo.Id);
            return releases.Select(r => _serviceProvider.CreateInstance<GitHubRelease>(r)).ToArray();
        }
    }
}
