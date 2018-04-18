using System;
using EnsureThat;

namespace Domain
{
    public sealed class Post : DomainBase
    {
        public Post(Guid id, Guid threadId, DateTime created, string name, string comment, bool isSage, string ipAddress) : base(id)
        {
            ThreadId = EnsureArg.IsNotEmpty(threadId, nameof(threadId));
            Created = EnsureArg.IsNotDefault(created, nameof(created));
            Name = EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
            Comment = EnsureArg.IsNotNullOrWhiteSpace(comment, nameof(comment));
            IsSage = isSage;
            IpAddress = EnsureArg.IsNotNull(ipAddress, nameof(ipAddress));
        }

        public Post()
        {
        }

        public Guid ThreadId { get; private set; }
        public DateTime Created { get; private set; }

        public string Name { get; private set; }
        public string Comment { get; private set; }
        public bool IsSage { get; private set; }
        public string IpAddress { get; private set; }
    }
}