using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ShitForum.Attributes;

namespace ShitForum.Models
{
    public class AddThread
    {
        public AddThread(Guid boardId, string name, string options, string subject, string comment, IFormFile file)
        {
            BoardId = boardId;
            Name = name;
            Options = options;
            Subject = subject;
            Comment = comment;
            File = file;
        }

        public AddThread()
        {
        }

        [Required]
        public Guid BoardId { get; set; }
        public string Name { get; set; }
        public string Options { get; set; }
        public string Subject { get; set; }
        [Required]
        public string Comment { get; set; }

        [ImageValidationAttribute]
        [Required]
        public IFormFile File { get; set; }
    }
}
