#nullable enable
using System;
using System.Collections.Generic;

namespace GithubService
{
    public class RepositoryAlreadyExistsException : Exception
    {
        public RepositoryAlreadyExistsException(string name) : base($@"Repository wth name ""{name}"" already exists")
        { 
            
        }
    }
    public interface IGitHubRepository
    {
        public Guid Create(string name, string content, Guid commit);

      
        public bool Update(Repository repository);
        public bool Delete(Guid id);
        
        public Repository? FindByName(string name);
        public Repository? FindById(Guid id);


    }
    
}