#nullable enable
using System;
using System.Collections.Concurrent;
using System.Linq;


namespace GithubService
{
    public class InMemoryListGitHubRepository : IGitHubRepository
    {
        private readonly ConcurrentDictionary<Guid, Repository> _repositories = new();


        public Guid Create(string name, string content, Guid commit)
        {
            var repo = _repositories.Values.FirstOrDefault(x => x.Name.Equals(name));
            var repoNew = new Repository(name)
            {
                Content =  content,
            };
            repoNew.Commits.Add(commit);
            if (!(repo == null && _repositories.TryAdd(repoNew.Id, repoNew)))
            {
                throw new RepositoryAlreadyExistsException(name);
            }

            return repoNew.Id;
        }


        public bool Update(Repository repository)
        {
            if (!_repositories.TryGetValue(repository.Id, out var current)) // && current == null)
            {
                return false;
            }
            
            return _repositories.TryUpdate(repository.Id, repository, current!);
            // return _repositories.TryGetValue(repository.Id, out var current) && _repositories.TryUpdate(repository.Id, repository, current!);
        }


        #region old

        // public Guid Create(string name)
        // {
        //     var repo = new Repository(name);
        //
        //     if (_repositories.FirstOrDefault(x => x.Name.Equals(name)) != null)
        //     {
        //         throw new Exception($@"Repository wth name ""{name}"" already exists");
        //     }
        //
        //     _repositories.Add(repo);
        //     return repo.Id;
        // }
        //
        // public bool Save(Guid id,string? name, Guid commit)
        // {
        //     var repo = _repositories.FirstOrDefault(x => x.Id.Equals(id));
        //     if (repo == null)
        //     {
        //         return false;
        //     }
        //
        //     if (name != null)
        //     {
        //         repo.Name = name;
        //     }
        //    
        //
        //     repo.Commits.Add(commit); 
        //
        //
        //     return true;
        // }

        #endregion

        public bool Delete(Guid id)
        {
            return _repositories.TryRemove(id, out _);
        }

        public Repository? FindByName(string name)
        {
            return _repositories.Values.FirstOrDefault(x => x.Name.Equals(name));


// throw new Exception($@Repository wth name ""{name}"" not exists");
        }

        public Repository? FindById(Guid id)
        {
            if (!_repositories.TryGetValue(id, out var repo) && repo == null)
            {
                return default;
            }

            return repo;
        }
    }
}