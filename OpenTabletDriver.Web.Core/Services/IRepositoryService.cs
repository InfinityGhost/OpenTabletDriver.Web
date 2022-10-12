using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Octokit;

namespace OpenTabletDriver.Web.Core.Services
{
    public interface IRepositoryService
    {
        Task<Repository> GetRepository();
        Task<string> DownloadRepositoryTarball(Repository repository);
        Task<FileStream> GetFile(Repository repository, string relativePath);
        Task<IEnumerable<FileStream>> GetFiles(Repository repository, Func<string, bool> predicate);
    }
}
