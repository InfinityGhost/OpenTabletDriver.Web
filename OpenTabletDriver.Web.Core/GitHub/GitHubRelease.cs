using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using OpenTabletDriver.Web.Core.Contracts;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Core.GitHub
{
    public class GitHubRelease : IRelease
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IGitHubClient _client;
        private readonly IRepositoryService _repositoryService;
        private readonly Release _release;

        public GitHubRelease(
            IServiceProvider serviceProvider,
            IGitHubClient client,
            IRepositoryService repositoryService,
            Release release
        )
        {
            _serviceProvider = serviceProvider;
            _client = client;
            _repositoryService = repositoryService;
            _release = release;
        }

        private IEnumerable<ReleaseAsset> _releaseAssets;

        public string Name => _release.TagName;
        public string Tag => _release.TagName;
        public string Url => _release.HtmlUrl;
        public string Body => _release.Body;
        public DateTimeOffset Date => _release.PublishedAt ?? _release.CreatedAt;

        public async Task<IEnumerable<IReleaseAsset>> GetReleaseAssets()
        {
            var repo = await _repositoryService.GetRepository();
            _releaseAssets ??= await _client.Repository.Release.GetAllAssets(repo.Id, _release.Id);
            return _releaseAssets.Select(r => _serviceProvider.CreateInstance<GitHubReleaseAsset>(r));
        }
    }
}
