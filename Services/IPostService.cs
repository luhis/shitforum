using Domain;
using Optional;
using System;
using System.Threading.Tasks;
using Services.Dtos;
using OneOf;
using Services.Results;

namespace Services
{
    public interface IPostService
    {
        Task<OneOf<Success, Banned, ImageCountExceeded, PostCountExceeded>> Add(Guid postId, Guid threadId, TripCodedName name, string comment, bool isSage, IpHash ipAddress, Option<File> file);

        Task<OneOf<Success, Banned>> AddThread(Guid postId, Guid threadId, Guid boardId, string subject, TripCodedName name, string comment, bool isSage, IpHash ipAddress, Option<File> file);
    }
}