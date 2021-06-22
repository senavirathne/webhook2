using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Packagist_Service
{
    public class InMemoryPackagistRepository : IPackagistRepository
    {
        private readonly ConcurrentDictionary<string, Package> _repositories = new();

        public string Add(string name, string organization, string gitHubUrl, Guid lastCommit)
        {
            var pac = new Package(name, organization, gitHubUrl, lastCommit);
            if (!_repositories.TryAdd(pac.GitHubUrl, pac))
            {
                throw new Exception($@"package wth Name ""{pac.Name}"" already exists");
            }

            return pac.GitHubUrl;
        }

        public bool Update(string name, Guid commit)
        {
            var repo = _repositories.Values.FirstOrDefault(x => x.Name.Equals(name));

            if (repo == null || !(_repositories.TryGetValue(repo!.Name, out var current))) // && current != //<=============
            {
                return false;
            }

            var package = current;
            package!.LastCommit = commit; //<======== other commits??
            return _repositories.TryUpdate(name, package, current!);
        }

        public bool Delete(string name)
        {
            return _repositories.TryRemove(name, out _);
        }

        public Package FindByName(string name)
        {
            return _repositories.Values.FirstOrDefault(x => x.Name.Equals(name));
        }
    }
}