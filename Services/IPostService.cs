using Domain;
using Optional;
using System;
using System.Threading.Tasks;
using Domain.IpHash;
using Services.Dtos;
using OneOf;
using Services.Results;

namespace Services
{
    public interface IPostService
    {
        Task<OneOf<Success, Banned, ImageCountExceeded, PostCountExceeded>> Add(Guid postId, Guid threadId, TripCodedName name, string comment, bool isSage, IIpHash ipAddress, Option<File> file);
        Task<Option<PostContextView>> GetById(Guid id);
        Task<OneOf<Success, Banned>> AddThread(Guid postId, Guid threadId, Guid boardId, string subject, TripCodedName name, string comment, bool isSage, IIpHash ipAddress, Option<File> file);
        Task<bool> DeletePost(Guid id);
    }
}
