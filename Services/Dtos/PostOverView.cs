using System;
using Domain;
using EnsureThat;
using Optional;

namespace Services.Dtos
{
    public sealed class PostOverView 
    {
        public PostOverView(Guid id, DateTime created, string name, string comment, Option<File> file, bool isSage, string ipAddress)
        {
            Id = EnsureArg.IsNotEmpty(id, nameof(id));
            Created = EnsureArg.IsNotDefault(created, nameof(created));
            Name = EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
            Comment = EnsureArg.IsNotNullOrWhiteSpace(comment, nameof(comment));
            File = file;
            IsSage = isSage;
            IpAddress = EnsureArg.IsNotNull(ipAddress, nameof(ipAddress));
        }

        public Guid Id { get; }
        public DateTime Created { get; }

        public string Name { get; }
        public string Comment { get; }
        public Option<File> File { get; }
        public bool IsSage { get; }
        public string IpAddress { get; }
    }
}