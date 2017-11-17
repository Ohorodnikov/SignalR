using SignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.ViewModel
{
    public class VoteViewModel
    {
        public List<ImageModel> Images { get; set; }
        public SessionUser User { get; set; }
    }
}
