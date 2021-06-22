using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Packagist_Service.Controllers
{
    public record PackageRequest(string Name, string Organization, string GitHubUrl);
    //public record UpdateCommit(Guid Commit);
    
    [ApiController]
    [Route("/Packages")]
    public class PackageController : Controller
    {
        private readonly ILogger<PackageController> _logger;
        private readonly PackageService _service;
        public PackageController(ILogger<PackageController> logger, PackageService service)
        {
            _logger = logger;
            _service = service;
        }
       
//post
        [HttpPost]
        public IActionResult Create([FromBody] PackageRequest packageRequest)
        {
            var packageDto = new PackageDto(packageRequest.Name, packageRequest.Organization);
            try
            {
                var gitHubUrl = _service.Create(packageDto);
          

                _logger.LogInformation("Package {Name} is added", packageRequest.Name);

                var response = new JsonResult(new {Name = packageRequest.Name, Url = gitHubUrl, Organization = packageRequest.Organization})
                {
                    StatusCode = (int) HttpStatusCode.Created
                };

                return response;
            }
            catch (RepositoryNotFoundException e)
            {
                _logger.LogInformation("{Message}",e.Message);
                return NotFound();
            }
        }

//get
        [Route("{name}")]
        [HttpGet]
        public IActionResult Get([FromRoute] string name)
        {
            var pacName = new ViewPackageDto(name).Name;
            var pac = _service.GetByName(pacName);
            if (pac == null)
            {
                return NotFound();
            }

            var response = new JsonResult(new {pac.Name, pac.Organization,pac.GitHubUrl})
            {
                StatusCode = (int) HttpStatusCode.Created
            };
            return response;
        }

        [Route("{name}/webhook")]
        [HttpPost]
        public async Task<IActionResult> Ping([FromRoute] string name)
        {
            //response pong
            var pacName = new ViewPackageDto(name).Name;
            if (await _service.WebHook(pacName)) //<===last commit
            {
                return NoContent();
            }

            return BadRequest();

        }


    }
}