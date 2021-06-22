using System;

namespace GithubService
{
    public class RepositoryService
    {
        private readonly IGitHubRepository _repository;

        public RepositoryService(IGitHubRepository repository)
        {
            _repository = repository;
        }

        public Guid CreateRepository(CreateRepositoryDto repositoryDto)
        {
            return _repository.Create(repositoryDto.Name,string.Empty,Guid.NewGuid());
        }

        public Repository GetRepository(string name)
        {
            return _repository.FindByName(name);
        }

        public bool UpdateRepository(Guid id, string content)
        {
            var repo = _repository.FindById(id);
            if (repo == null)
            {
                return false;
            }

            repo.Content = content;
            repo.Commits.Add(Guid.NewGuid());
            return _repository.Update(repo);
        }
    }
}