using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace GithubService.Controllers
{
    public record RepositoryRequest (string Name);
    public record EditRepositoryRequest (string Content);

    [ApiController]
    [Route("/Repositories")]
    public class RepositoryController : Controller
    {
        private readonly ILogger<RepositoryController> _logger;
        private readonly RepositoryService _repositoryService;

        public RepositoryController(ILogger<RepositoryController> logger, RepositoryService repositoryService)
        {
            _logger = logger;
            _repositoryService = repositoryService;
        }

//get
        [Route("{Name}")]
        [HttpGet]
        public IActionResult Get([FromRoute] string name)
        {
            var repository = _repositoryService.GetRepository(name);
            if (repository == null)
            {
                return NotFound();
            }

            return new JsonResult(new RepositoryViewModel(repository.Name, repository.Commits.Last())); //for optimization
            // return new JsonResult(new { repository.Name, LastCommit = repository.Commits.Last()});
//post
        }

        [HttpPost]
        public IActionResult Create([FromBody] RepositoryRequest request) //model binding (mapping)
        {
            var repository = new CreateRepositoryDto(request.Name);
            try
            {
                var repositoryId = _repositoryService.CreateRepository(repository);
                _logger.LogInformation("Repository Name =>{Name}", request.Name);
                _logger.LogInformation("Created Repository Id =>{Id}", repositoryId);
                
                return new JsonResult(new {Id = repositoryId, Name = request.Name})
                    {StatusCode = (int) HttpStatusCode.Created};
                
            }
            catch (RepositoryAlreadyExistsException e)
            {
                _logger.LogInformation("{Message}",e.Message);
                
            }
            return BadRequest();
           
        }
        [Route("{id:guid}")]
        [HttpPatch]
        public IActionResult Edit([FromBody] EditRepositoryRequest request, [FromRoute] Guid id)
        {
            if ( _repositoryService.UpdateRepository(id, request.Content))
            {
                return NoContent();
            }

            return BadRequest();
        }
    }
}