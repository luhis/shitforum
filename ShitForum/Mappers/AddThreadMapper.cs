using System;
using Domain;
using Domain.IpHash;
using Services;
using Services.Dtos;
using ShitForum.Models;

namespace ShitForum.Mappers
{
    public static class AddThreadMapper
    {
        public static Thread Map(AddThread t) => new Thread(Guid.NewGuid(), t.BoardId, StringFuncs.MapString(t.Subject, string.Empty));

        public static Domain.Post MapToPost(AddThread t, Guid threadId, TripCodedName name, IpHash ipAddress)
        {
            var options = OptionsMapper.Map(t.Options);
            return new Domain.Post(Guid.NewGuid(), threadId, DateTime.UtcNow, name.Val,
                t.Comment, options.Sage, ipAddress.Val);
        }
    }
}