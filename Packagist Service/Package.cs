using System;

namespace Packagist_Service
{
    public record PackageDto(string Name, string Organization);

    public record ViewPackageDto(string Name);
    public class Package
    {
        public Guid LastCommit { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string GitHubUrl { get; set; }

        public Package(string name, string organization, string gitHubUrl, Guid lastCommit)
        {
            Name = name;
            Organization = organization;
            GitHubUrl = gitHubUrl;
            LastCommit = lastCommit;
        }
    }
}