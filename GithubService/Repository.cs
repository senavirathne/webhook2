using System;
using System.Collections.Generic;

namespace GithubService
{
    public record CreateRepositoryDto(string Name);

    public record RepositoryDto(Guid Id, string Name, List<Guid> Commits);

    public record RepositoryViewModel(string Name, Guid LastCommit);

    public class Repository
    {
        
        public Repository(string name)
        {   
            Id = Guid.NewGuid();
            Name = name;
            //Content = string.Empty;
            Commits = new(); //List<Guid> {Guid.NewGuid()};
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> Commits { get; set; }
        public string Content { get; set; }

        private bool Equals(Repository other)
        {
            return Id.Equals(other.Id) && Name == other.Name && Equals(Commits, other.Commits);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Repository) obj);
        }

    }
}