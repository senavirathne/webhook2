#nullable enable
using System;

namespace Packagist_Service
{
    public interface IPackagistRepository
    {
        public string Add(string name, string organization, string gitHubUrl, Guid lastCommit);

        public bool Update(string name, Guid commit);

        public bool Delete(string name);
        public Package? FindByName(string name);
       
    }
}