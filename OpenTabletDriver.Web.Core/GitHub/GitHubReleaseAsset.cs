using Octokit;
using OpenTabletDriver.Web.Core.Contracts;

namespace OpenTabletDriver.Web.Core.GitHub
{
    public class GitHubReleaseAsset : IReleaseAsset
    {
        private readonly ReleaseAsset _asset;

        public GitHubReleaseAsset(ReleaseAsset asset)
        {
            _asset = asset;
        }

        public string FileName => _asset.Name;
        public string Url => _asset.BrowserDownloadUrl;
    }
}
