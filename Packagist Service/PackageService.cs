using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Packagist_Service
{
    public abstract class GitHubRepository
    {
        public string Name { get; set; }
        public string LastCommit { get; set; }
    }

    public class RepositoryNotFoundException : Exception
    {
        public RepositoryNotFoundException(string name) : base($"Repository {name} doesn't exists")
        {
        }
    }

    public class PackageService
    {
        private readonly IPackagistRepository _repository;
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<PackageService> _logger;

        public PackageService(IPackagistRepository repository, IHttpClientFactory factory,
            ILogger<PackageService> logger)
        {
            _repository = repository;
            _factory = factory;
            _logger = logger;
        }

        public async Task<string> Create(PackageDto packageDto)
        {
            var (name, organization) = packageDto;
            var client = _factory.CreateClient("GitHub");
            using var response = await client.GetAsync($"/Repositories/{name}"); // <=======

            // using var client = new HttpClient()
            // {
            //     BaseAddress = new Uri("https://localhost:5002/Repositories/"),
            //     Timeout = TimeSpan.FromSeconds(5)
            //     
            // };
            // using var response = await client.GetAsync($"{name}");
            if (!response.IsSuccessStatusCode)
            {
                throw new RepositoryNotFoundException(name); //  <=====dispose
            }

            var content = await response.Content.ReadFromJsonAsync<GitHubRepository>();

            return _repository.Add(name, organization,
                String.Concat(client.BaseAddress!.ToString(), $"/Repositories/{content!.Name}"), //<====== =name
                Guid.Parse(content!.LastCommit));
        }

        public async Task<bool> WebHook(string name)
        {
            var client = _factory.CreateClient("GitHub");
            using var response = await client.GetAsync($"/Repositories/{name}"); 
            if (!response.IsSuccessStatusCode)
            {
                throw new RepositoryNotFoundException(name); 
            }
            var content = await response.Content.ReadFromJsonAsync<GitHubRepository>();

            return _repository.Update(content!.Name, Guid.Parse(content.LastCommit));
        }

        public Package GetByName(string name)
        {
            return _repository.FindByName(name);
        }
    }
}