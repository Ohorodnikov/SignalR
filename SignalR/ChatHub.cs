using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalR.Controllers;
using SignalR.Data;
using SignalR.Models;
using SignalR.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalR
{
    public class ChatHub : Hub
    {
        private IDBService _service;
        public ChatHub(IDBService service)
        {
            _service = service;
        }
        public override Task OnConnectedAsync()
        {            
            return base.OnConnectedAsync();
        }

         

        public async Task ReRegister(string sessionCode)
        {
            var sessionUser = _service.GetSessionUser(sessionCode, GetUserId());
            sessionUser.ConnectionId = Context.ConnectionId;
            _service.UpdateSessionUser(sessionUser);
            await Groups.AddAsync(Context.ConnectionId, sessionCode);
        }

        public string GetUserId()
        {
            var claimsIdentity = Context.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;
            return userId;
            //return userId != null ? UserManager.GetUserByID(Guid.Parse(userId)) : null;
        }

        public async Task Start()
        {
            var _user = new SessionUser()
            {
                ConnectionId = Context.ConnectionId,
                IsTeacher = true,
                UserId = GetUserId()
            };

            
            var sessionCode = GenerateSessionCode(5);
            var session = new Session()
            {
                Users = new List<SessionUser>(),
                SessionCode = sessionCode
            };
            session.Users.Add(_user);
            _service.SaveSession(session);
            //context.Session.Add(session);
            //context.SaveChanges();
            await Groups.AddAsync(Context.ConnectionId, sessionCode);

            await Clients.Client(Context.ConnectionId).InvokeAsync("SessionCode", sessionCode);
        }
        
        private string GenerateSessionCode(int length)
        {
            var code = "";
            for (int i = 0; i < length; i++)
            {
                code += (char)new Random().Next(65, 90);
            }
            return code;   
        }

        public async Task JoinGroup(string sessionCode)
        {   
            var session = _service.SearchSession(sessionCode);
            var users = _service.SearchSessionUsers(session.Id).ToList();
            session.Users = users;
            if (session != null)
            {
                var user = new SessionUser()
                {
                    UserId = GetUserId(),
                    ConnectionId = Context.ConnectionId,
                    IsTeacher = false
                };
                
                session.Users.Add(user);
                _service.SaveSessionUser(user);
                user.User = _service.GetUser(user.UserId);
                await Groups.AddAsync(Context.ConnectionId, sessionCode);
                await Clients.Group(sessionCode).InvokeAsync("Join", $"{user.User.Email} joined.");
            }
                        
        }

        public async Task AddLike(string sessionCode, string imgId)
        {
            await Clients.Group(sessionCode).InvokeAsync("Like", imgId);
        }

        public async Task AddDislike(string sessionCode, string imgId)
        {
            await Clients.Group(sessionCode).InvokeAsync("Dislike", imgId);
        }

        public async Task StartVoting(string sessionCode)
        {
            await Clients.Group(sessionCode).InvokeAsync("Voting", sessionCode);
        }

        public async Task Send(string message, string name)
        {
            await Clients.All.InvokeAsync("Sent", message, name);
        }

        public async Task SendFile(string sessionCode, string base64, string id)
        {
            await Clients.Group(sessionCode).InvokeAsync("File", base64, id);
        }

        public async Task UpdateCounter(string sessionCode, string newValue)
        {
            await Clients.Group(sessionCode).InvokeAsync("Update", newValue);
        }
    }
}
