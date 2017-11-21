using Moq;
using SignalR.Data;
using SignalR.Models;
using SignalR.Services;
using System;
using System.Linq;
using Xunit;

namespace SignalR.Test.NetCore
{
    public class UnitTest1
    {
        [Fact]
        public void SearchSessionBySessionCode()
        {
            using(var context = Context.GetContextWithData())
            {
                var service = new DBService(context);

                var result = service.SearchSession("CODE1");                
                Assert.NotNull(result);
                Assert.True(result.Id == 1);
            }
        }

        [Fact]
        public void TestUser1ImagesCount()
        {
            using (var context = Context.GetContextWithData())
            {
                var service = new DBService(context);

                var result = service.GetUserImages("user1").ToList();
                Assert.NotNull(result);
                Assert.True(result.Count == 3);
            }
        }

        [Fact]
        public void TestSaveSession()
        {
            using (var context = Context.GetContextWithData())
            {
                var service = new DBService(context);
                var session = new Session
                {
                    Id = 3,
                    SessionCode = "CODE3"
                };
                service.SaveSession(session);
                var result = service.SearchSession(session.SessionCode);
                Assert.NotNull(result);
                Assert.True(result.Id == session.Id);
            }
        }

        [Fact]
        public void TestSaveNullSession()
        {
            using (var context = Context.GetContextWithData())
            {
                var service = new DBService(context);
                Session session = null;               
                
                Assert.Throws<ArgumentException>(() => service.SaveSession(session));                
            }
        }

        [Fact]
        public void TestVotedUsersForImageCount()
        {
            using (var context = Context.GetContextWithData())
            {
                var service = new DBService(context);

                var result = service.GetVotedUsersForImage(1, 1).ToList();

                Assert.True(2 == result.Count);
            }
        }

        [Fact]
        public void TestVotingImage()
        {
            using (var context = Context.GetContextWithData())
            {
                var service = new DBService(context);

                var prevRes = service.GetImageVotesCount(1, 1);
                service.VoteImg(1, 5, 1, true);
                var newRes = service.GetImageVotesCount(1, 1);
                Assert.True(newRes - prevRes == 1);
            }
        }

        [Fact]
        public void TestSearchSessionUsers()
        {
            using (var context = Context.GetContextWithData())
            {
                var service = new DBService(context);

                var result = service.SearchSessionUsers(1);

                Assert.True(3 == result.Count());
            }
        }

        [Fact]
        public void TestSaveSessionUser()
        {
            using (var context = Context.GetContextWithData())
            {
                var service = new DBService(context);

                var sessionUser = new SessionUser
                {
                    ConnectionId = "CONNECTION_6",
                    Id = 6,
                    IsTeacher = false,
                    SessionId = 2,
                    UserId = "user3"
                };

                service.SaveSessionUser(sessionUser);
                var result = service.GetSessionUser("CODE2", "user3");
                Assert.NotNull(result);
                Assert.True(result.Id == sessionUser.Id && result.SessionId == sessionUser.SessionId);
            }
        }
       
    }
}
