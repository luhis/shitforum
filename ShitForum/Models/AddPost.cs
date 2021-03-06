﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ShitForum.Attributes;

namespace ShitForum.Models
{
    public class AddPost
    {
        public AddPost()
        {
        }

        public AddPost(Guid threadId, string name, string options, string comment, IFormFile file)
        {
            ThreadId = threadId;
            Name = name;
            Options = options;
            Comment = comment;
            File = file;
        }

        [Required]
        public Guid ThreadId { get; set; }
        public string Name { get; set; }
        public string Options { get; set; }
        [Required]
        public string Comment { get; set; }

        [ImageValidation]
        public IFormFile File { get; set; }
    }
}
