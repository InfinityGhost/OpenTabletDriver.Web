using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Core.GitHub.Services
{
    public class GitHubTabletService : ITabletService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IReleaseService releaseService;

        public GitHubTabletService(IServiceProvider serviceProvider, IReleaseService releaseService)
        {
            this.serviceProvider = serviceProvider;
            this.releaseService = releaseService;

            GetMarkdownCached = new CachedTask<string>(GetMarkdownRawInternal, CacheTime);
        }

        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(1);

        private CachedTask<string> GetMarkdownCached { get; }

        public Task<string> GetMarkdownRaw()
        {
            return GetMarkdownCached;
        }

        public async Task<string> GetMarkdownRawInternal()
        {
            var repoContent = await releaseService.GetRepositoryContent();
            var file = repoContent.First(r => r.Name == "TABLETS.md");

            using (var httpClient = serviceProvider.GetRequiredService<HttpClient>())
            using (var httpStream = await httpClient.GetStreamAsync(file.DownloadUrl))
            using (var sr = new StreamReader(httpStream))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}