using System;
using Domain;
using Services.Dtos;
using ShitForum.Models;

namespace ShitForum.Mappers
{
    public static class AddPostMapper
    {
        public static Domain.Post Map(AddPost post, TripCodedName name, IpHash ipAddress)
        {
            var options = OptionsMapper.Map(post.Options);
            return new Domain.Post(Guid.NewGuid(), post.ThreadId, DateTime.UtcNow,
                name.Val, post.Comment, options.Sage,
                ipAddress.Val);
        }
    }
}