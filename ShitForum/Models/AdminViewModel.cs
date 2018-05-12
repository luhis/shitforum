using System.Collections.Generic;
using Domain;
using EnsureThat;

namespace ShitForum.Models
{
    public class AdminViewModel
    {
        public AdminViewModel(string message, bool isAdmin, IReadOnlyList<BannedImage> bannedImages, IReadOnlyList<BannedIp> bannedUsers)
        {
            Message = EnsureArg.IsNotNull(message, nameof(message));
            IsAdmin = isAdmin;
            BannedImages = EnsureArg.IsNotNull(bannedImages, nameof(bannedImages));
            BannedUsers = EnsureArg.IsNotNull(bannedUsers, nameof(bannedUsers));
        }

        public string Message { get; }
        public bool IsAdmin { get; }
        public IReadOnlyList<BannedImage> BannedImages { get; }
        public IReadOnlyList<BannedIp> BannedUsers { get; }
    }
}
