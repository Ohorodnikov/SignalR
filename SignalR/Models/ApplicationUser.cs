using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SignalR.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public List<ImageModel> Images { get; set; }
        public List<SessionUser> SessionUsers { get; set; }
        public string ParentUserId { get; set; }
    }
}
