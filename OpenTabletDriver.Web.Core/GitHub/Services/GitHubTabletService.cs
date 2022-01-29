using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Internal;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Core.GitHub.Services
{
    public class GitHubTabletService : ITabletService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IReleaseService releaseService;
        private readonly HttpClient httpClient;

        public GitHubTabletService(IServiceProvider serviceProvider, IReleaseService releaseService, HttpClient httpClient)
        {
            this.serviceProvider = serviceProvider;
            this.releaseService = releaseService;
            this.httpClient = httpClient;

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
            using (var httpStream = await httpClient.GetStreamAsync("https://raw.githubusercontent.com/OpenTabletDriver/OpenTabletDriver/master/TABLETS.md"))
            using (var sr = new StreamReader(httpStream))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}