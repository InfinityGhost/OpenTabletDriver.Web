using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Core.GitHub.Services
{
    public class GitHubTabletService : ITabletService
    {
        private readonly IServiceProvider serviceProvider;

        public GitHubTabletService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            GetMarkdownCached = new CachedTask<string>(GetMarkdownRawInternal(), CacheTime);
        }

        private const string TABLETS_MARKDOWN_URL =
            "https://github.com/OpenTabletDriver/OpenTabletDriver/raw/master/TABLETS.md";
        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(1);

        private CachedTask<string> GetMarkdownCached { get; }

        public Task<string> GetMarkdownRaw()
        {
            return GetMarkdownCached;
        }

        public async Task<string> GetMarkdownRawInternal()
        {
            using (var client = serviceProvider.GetRequiredService<HttpClient>())
            using (var httpStream = await client.GetStreamAsync(TABLETS_MARKDOWN_URL))
            using (var sr = new StreamReader(httpStream))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}