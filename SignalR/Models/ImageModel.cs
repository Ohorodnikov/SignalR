using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string Base64 { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }

        public List<VoteInfo> VotedUsers { get; } = new List<VoteInfo>();

        public string ApplicationUserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
