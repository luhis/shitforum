using System;
using EnsureThat;

namespace Domain
{
    public abstract class DomainBase
    {
        protected DomainBase(Guid id)
        {
            Id = EnsureArg.IsNotEmpty(id, nameof(id));
        }

        protected DomainBase()
        {
        }

        public Guid Id { get; private set; }
    }
}