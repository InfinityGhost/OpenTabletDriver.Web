using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Octokit;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Core.Plugins
{
    public class GitHubPluginMetadataService : IPluginMetadataService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IGitHubClient _client;
        private readonly IMemoryCache _cache;

        public GitHubPluginMetadataService(
            IRepositoryService repositoryService,
            IGitHubClient client,
            IMemoryCache cache
        )
        {
            _repositoryService = repositoryService;
            _client = client;
            _cache = cache;
        }

        private const string REPOSITORY_OWNER = "OpenTabletDriver";
        private const string REPOSITORY_NAME = "Plugin-Repository";

        private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(10);

        public Task<IReadOnlyList<PluginMetadata>> GetPlugins()
        {
            const string key = nameof(GitHubPluginMetadataService) + nameof(GetPlugins);
            return _cache.GetOrCreateAsync(key, GetPluginsInternal);
        }

        private async Task<IReadOnlyList<PluginMetadata>> GetPluginsInternal(ICacheEntry entry)
        {
            entry.AbsoluteExpirationRelativeToNow = Expiration;

            var plugins = await DownloadAsync(REPOSITORY_OWNER, REPOSITORY_NAME);
            return plugins.OrderBy(p => p.Name).Distinct(new PluginMetadataComparer()).ToArray();
        }

        private async Task<IEnumerable<PluginMetadata>> DownloadAsync(string owner, string name)
        {
            var repo = await _client.Repository.Get(owner, name);
            var path = await _repositoryService.DownloadRepositoryTarball(repo);
            return EnumeratePluginMetadata(path);
        }

        private static IEnumerable<PluginMetadata> EnumeratePluginMetadata(string directoryPath)
        {
            var serializer = new JsonSerializer();
            foreach (var file in Directory.EnumerateFiles(directoryPath, "*.json", SearchOption.AllDirectories))
            {
                using (var fs = File.OpenRead(file))
                using (var sr = new StreamReader(fs))
                using (var jr = new JsonTextReader(sr))
                    yield return serializer.Deserialize<PluginMetadata>(jr);
            }
        }

        private class PluginMetadataComparer : IEqualityComparer<PluginMetadata>
        {
            public bool Equals(PluginMetadata x, PluginMetadata y)
            {
                if (x is null || y is null)
                    return false;

                return x.Name == y.Name &&
                       x.Owner == y.Owner &&
                       x.RepositoryUrl == y.RepositoryUrl;
            }

            public int GetHashCode(PluginMetadata obj)
            {
                return HashCode.Combine(obj.Name, obj.Owner, obj.RepositoryUrl);
            }
        }
    }
}
