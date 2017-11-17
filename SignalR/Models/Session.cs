using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string SessionCode { get; set; }
        public ICollection<SessionUser> Users { get; set; } = new List<SessionUser>();
        public ICollection<VoteInfo> Votes { get; set; } = new List<VoteInfo>();
    }
}
