using System;
using System.Threading.Tasks;

namespace GithubService
{
    public class RepositoryService
    {
        private readonly IGitHubRepository _repository;
        private readonly PackagistService _packagistService;

        public RepositoryService(IGitHubRepository repository, PackagistService packagistService)
        {
            _repository = repository;
            _packagistService = packagistService;
        }

        public Guid CreateRepository(CreateRepositoryDto repositoryDto)
        {
            return _repository.Create(repositoryDto.Name, string.Empty, Guid.NewGuid());
        }

        public Repository GetRepository(string name)
        {
            return _repository.FindByName(name);
        }

        public async Task<bool> UpdateRepository(string name, string content)
        {
            var repo = _repository.FindByName(name);

            if (repo == null)
            {
                return false;
            }

            repo.Content = content;
            repo.Commits.Add(Guid.NewGuid());

            var isUpdated = _repository.Update(repo);
            if (isUpdated)
            {
                // Channel => write repo.name
                // await _packagistService.Writer.WriteAsync(repo.Name);
                await _packagistService.Notify(repo.Name);
            }

            return isUpdated;
        }
    }
}