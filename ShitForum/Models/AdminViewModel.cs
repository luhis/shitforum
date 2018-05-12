using System.Collections.Generic;
using Domain;
using EnsureThat;
using Optional;

namespace ShitForum.Models
{
    public class AdminViewModel
    {
        public AdminViewModel(string message, Option<IReadOnlyList<BannedImage>> bannedImages, Option<IReadOnlyList<BannedIp>> bannedUsers)
        {
            Message = EnsureArg.IsNotNull(message, nameof(message));
            BannedImages = EnsureArg.IsNotNull(bannedImages, nameof(bannedImages));
            BannedUsers = EnsureArg.IsNotNull(bannedUsers, nameof(bannedUsers));
        }

        public string Message { get; }
        public Option<IReadOnlyList<BannedImage>> BannedImages { get; }
        public Option<IReadOnlyList<BannedIp>> BannedUsers { get; }
    }
}
