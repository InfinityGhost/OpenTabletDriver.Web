using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTabletDriver.Web.Core.Services;

namespace OpenTabletDriver.Web.Core.GitHub.Services
{
    public class GitHubTabletService : ITabletService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IReleaseService releaseService;
        private readonly ILogger<GitHubTabletService> logger;

        public GitHubTabletService(
            IServiceProvider serviceProvider,
            IReleaseService releaseService,
            ILogger<GitHubTabletService> logger
        )
        {
            this.serviceProvider = serviceProvider;
            this.releaseService = releaseService;
            this.logger = logger;

        }

        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(1);

        public Task<string> GetMarkdownRaw()
        {
            return GetMarkdownRawInternal();
        }

        public async Task<string> GetMarkdownRawInternal()
        {
            logger.Log(LogLevel.Information, "Fetching Tablets table...");

            var repoContent = await releaseService.GetRepositoryContent();
            logger.Log(LogLevel.Information, "Fetched repository content");

            var file = repoContent.First(r => r.Name == "TABLETS.md");
            logger.Log(LogLevel.Information, $"Found TABLETS.md: {file.Size} bytes at {file.DownloadUrl}");

            using (var httpClient = new HttpClient())
            {
                logger.Log(LogLevel.Information, "Downloading TABLETS.md...");
                var content = await httpClient.GetStringAsync(file.DownloadUrl);

                logger.Log(LogLevel.Information, "Successfully downloaded TABLETS.md");
                return content;
            }
        }
    }
}