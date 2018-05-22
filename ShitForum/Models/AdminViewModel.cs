using System.Collections.Generic;
using Domain;
using EnsureThat;

namespace ShitForum.Models
{
    public class AdminViewModel
    {
        public AdminViewModel(IReadOnlyList<BannedImage> bannedImages, IReadOnlyList<BannedIp> bannedUsers, IReadOnlyList<Board> boards)
        {
            BannedImages = EnsureArg.IsNotNull(bannedImages, nameof(bannedImages));
            BannedUsers = EnsureArg.IsNotNull(bannedUsers, nameof(bannedUsers));
            Boards = EnsureArg.IsNotNull(boards, nameof(boards));
        }
        
        public IReadOnlyList<BannedImage> BannedImages { get; }
        public IReadOnlyList<BannedIp> BannedUsers { get; }
        public IReadOnlyList<Board> Boards { get; }
    }
}
