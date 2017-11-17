using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Models
{
    public class VoteInfo
    {
        public int UserId { get; set; }
        public SessionUser User { get; set; }

        public int ImageId { get; set; }
        public ImageModel Image { get; set; }

        public bool Status { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}
