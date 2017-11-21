using Microsoft.EntityFrameworkCore;
using SignalR.Data;
using SignalR.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalR.Test.NetCore
{
    static class Context
    {
        public static ApplicationDbContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;

            var context = new ApplicationDbContext(options);
            context.Session.AddRange(
                new Session
                {
                    Id = 1,
                    SessionCode = "CODE1"
                },
                new Session
                {
                    Id = 2,
                    SessionCode = "CODE2"
                });

            context.ImageModel.AddRange(
                new ImageModel
                {
                    Id = 1,
                    Base64 = "BASE64_1",
                    LikesCount = 1,
                    DislikesCount = 1,
                    ApplicationUserId = "user1"
                },
                new ImageModel
                {
                    Id = 2,
                    Base64 = "BASE64_2",
                    LikesCount = 2,
                    DislikesCount = 2,
                    ApplicationUserId = "user1"
                },
                new ImageModel
                {
                    Id = 3,
                    Base64 = "BASE64_3",
                    LikesCount = 3,
                    DislikesCount = 3,
                    ApplicationUserId = "user1"
                });


            context.SessionUser.AddRange(
                new SessionUser
                {
                    Id = 1,
                    ConnectionId = "CONNECTION_1",
                    IsTeacher = true,
                    SessionId = 1,
                    UserId = "user1"
                },
                new SessionUser
                {
                    Id = 2,
                    ConnectionId = "CONNECTION_2",
                    IsTeacher = true,
                    SessionId = 2,
                    UserId = "user1"
                },
                new SessionUser
                {
                    Id = 3,
                    ConnectionId = "CONNECTION_3",
                    IsTeacher = false,
                    SessionId = 1,
                    UserId = "user2"
                },
                new SessionUser
                {
                    Id = 4,
                    ConnectionId = "CONNECTION_5",
                    IsTeacher = false,
                    SessionId = 1,
                    UserId = "user3"
                }, 
                new SessionUser
                {
                    Id = 5,
                    ConnectionId = "CONNECTION_5",
                    IsTeacher = false,
                    SessionId = 2,
                    UserId = "user1"
                });

            context.VoteInfo.AddRange(
                new VoteInfo
                {
                    ImageId = 1,
                    SessionId = 1,
                    UserId = 3,
                    Status = true
                },
                new VoteInfo
                {
                    ImageId = 2,
                    SessionId = 1,
                    UserId = 3,
                    Status = true
                },
                new VoteInfo
                {
                    ImageId = 1,
                    SessionId = 2,
                    UserId = 3,
                    Status = true
                },
                new VoteInfo
                {
                    ImageId = 1,
                    SessionId = 1,
                    UserId = 4,
                    Status = false
                });

            context.SaveChanges();
            return context;
        }
    }
}
