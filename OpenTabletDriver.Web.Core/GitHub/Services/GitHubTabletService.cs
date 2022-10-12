using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using OpenTabletDriver.Tablet;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Core.GitHub.Services
{
    public class GitHubTabletService : ITabletService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IMemoryCache _cache;

        public GitHubTabletService(
            IRepositoryService repositoryService,
            IMemoryCache cache
        )
        {
            _repositoryService = repositoryService;
            _cache = cache;
        }

        private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(10);

        public Task<IList<TabletConfiguration>> GetConfigurations()
        {
            const string key = nameof(GitHubTabletService) + nameof(GetConfigurations);
            return _cache.GetOrCreateAsync(key, GetConfigurationsInternal);
        }

        private async Task<IList<TabletConfiguration>> GetConfigurationsInternal(ICacheEntry entry)
        {
            entry.AbsoluteExpirationRelativeToNow = Expiration;

            var repo = await _repositoryService.GetRepository();
            var files = await _repositoryService.GetFiles(repo,
                f => f.StartsWith("OpenTabletDriver.Configurations/Configurations") && f.EndsWith(".json")
            );

            var serializer = new JsonSerializer();
            var list = new List<TabletConfiguration>();

            foreach (var file in files)
            {
                using (var sr = new StreamReader(file))
                using (var jr = new JsonTextReader(sr))
                {
                    var config = serializer.Deserialize<TabletConfiguration>(jr);
                    list.Add(config);
                }
            }

            return list;
        }
    }
}
