using Microsoft.EntityFrameworkCore;
using SignalR.Data;
using SignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Services
{
    public class DBService : IDBService
    {
        ApplicationDbContext _context;
        public DBService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool SaveSession(Session session)
        {
            if (session == null || string.IsNullOrEmpty(session.SessionCode))
            {
                throw new ArgumentException();
            }
            _context.Session.Add(session);
            _context.SaveChanges();
            return true;
        }

        public void SaveSessionUser(SessionUser sessionUser)
        {
            _context.SessionUser.Add(sessionUser);
            _context.SaveChanges();

        }
        public void UpdateSessionUser(SessionUser sessionUser)
        {
            _context.SessionUser.Update(sessionUser);
            _context.SaveChanges();
        }

        public void SaveUser(ApplicationUser user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

        }

        public Session SearchSession(string sessionCode)
        {
            return _context.Session.FirstOrDefault(s => s.SessionCode == sessionCode);
        }

        public IEnumerable<SessionUser> SearchSessionUsers(int sessionId)
        {
            return _context.SessionUser.Where(u => u.SessionId == sessionId);
        }

        public IEnumerable<VoteInfo> GetVotedUsersForImage(int imageId, int sessionId)
        {             
            return _context.ImageModel
                .Include(v => v.VotedUsers)
                .FirstOrDefault(i => i.Id == imageId)
                .VotedUsers
                .ToList()
                .Where(u => u.SessionId == sessionId);
        }

        public IEnumerable<VoteInfo> GetVotedImagesByUser(int userId)
        {
            return _context.SessionUser.Include(u => u.VotedImages).FirstOrDefault(u => u.Id == userId).VotedImages;
        }

        public IEnumerable<ImageModel> GetUserImages(string userId)
        {
            return _context.ImageModel.Where(u => u.ApplicationUserId == userId);
        }

        public ImageModel GetImage(int imgId)
        {
            return _context.ImageModel.Find(imgId);
        }

        public void RemoveImage(int imgId)
        {
            _context.Remove(GetImage(imgId));
            _context.SaveChanges();
        }

        public void VoteImg(int imgId, int userId, int sessionId, bool like)
        {
            _context.Add(new VoteInfo()
            {
                ImageId = imgId,
                UserId = userId,
                SessionId = sessionId,
                Status = like
            });
            _context.SaveChanges();
        }

        public int GetImageVotesCount(int imgId, int sessionId)
        {
            var x = GetVotedUsersForImage(imgId, sessionId);
            return x.Where(i => i.Status).Count();
        }
        public SessionUser GetSessionUser(string sessionCode, string userId)
        {
            return _context.SessionUser.FirstOrDefault(u => u.SessionId == SearchSession(sessionCode).Id && u.UserId == userId);
        }

        public ApplicationUser GetUser(string userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

    }

    public interface IDBService
    {
        void RemoveImage(int imgId);
        ImageModel GetImage(int imgId);
        void SaveUser(ApplicationUser user);
        bool SaveSession(Session session);
        void SaveSessionUser(SessionUser sessionUser);
        Session SearchSession(string sessionCode);
        IEnumerable<SessionUser> SearchSessionUsers(int sessionId);
        IEnumerable<VoteInfo> GetVotedUsersForImage(int imageId, int sessionId);
        IEnumerable<VoteInfo> GetVotedImagesByUser(int userId);
        IEnumerable<ImageModel> GetUserImages(string userId);
        int GetImageVotesCount(int imgId, int sessionId);
        void VoteImg(int imgId, int userId, int sessionId, bool like);
        SessionUser GetSessionUser(string sessionCode, string userId);
        ApplicationUser GetUser(string userId);
        void UpdateSessionUser(SessionUser sessionUser);
    }
}
