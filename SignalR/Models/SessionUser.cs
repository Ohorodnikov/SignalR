using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class SessionUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public int SessionId { get; set; }
        public bool IsTeacher { get; set; }

        public List<VoteInfo> VotedImages { get; } = new List<VoteInfo>();

        public Session Session { get; set; }
        public ApplicationUser User { get; set; }
    }
}
