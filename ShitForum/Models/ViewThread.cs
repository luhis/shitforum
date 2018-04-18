using Services;
using System;
using System.Collections.Generic;
using Services.Dtos;

namespace ShitForum.Models
{
    public class ViewThread
    {
        public ViewThread(Guid threadId, string subject, IEnumerable<PostOverView> posts)
        {
            ThreadId = threadId;
            Subject = subject;
            Posts = posts;
        }

        public Guid ThreadId { get; }
        public string Subject { get; }
        public IEnumerable<PostOverView> Posts { get; }
    }
}