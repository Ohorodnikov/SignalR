using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SignalR.Models;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using SignalR.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SignalR.ViewModel;
using System.Threading.Tasks;
using SignalR.Services;
using System.Collections.Generic;

namespace SignalR.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly UserManager<ApplicationUser> _manager;
        private readonly IDBService _dBService;
        public HomeController(IHostingEnvironment appEnvironment, UserManager<ApplicationUser> manager, IDBService dBService)
        {
            _appEnvironment = appEnvironment;

            _manager = manager;

            _dBService = dBService;
        }
        
        
        public IActionResult Voting(string sessionCode)
        {
            var user = GetCurrentUser();
            var sessionUser = _dBService.GetSessionUser(sessionCode, user.Id);
            sessionUser.Session = _dBService.SearchSession(sessionCode);
            var sessionId = sessionUser.Session.Id;
            var roles = _manager.GetRolesAsync(user).GetAwaiter().GetResult().ToList();
            if (roles.Contains(Role.Teacher.ToString()))
            {
                var notVoted = new List<ImageModel>();
                var images = _dBService.GetUserImages(user.Id).ToList();
                foreach (var image in images)
                {                   
                    image.LikesCount = _dBService.GetImageVotesCount(image.Id, sessionId);
                    image.DislikesCount = _dBService.GetVotedUsersForImage(image.Id, sessionId).Count() - image.LikesCount;
                    notVoted.Add(image);                    
                }
                var voteViewModel = new VoteViewModel()
                {
                    Images = notVoted,
                    User = sessionUser
                };
                return View("TeacherVoting", voteViewModel);
            }
            else
            {
                var allImages = _dBService.GetUserImages(user.ParentUserId).ToList();
                var notVoted = new List<ImageModel>();
                foreach (var image in allImages)
                {
                    var votedUsers = _dBService.GetVotedUsersForImage(image.Id, sessionId).Select(u => u.UserId).ToList();
                    if (!votedUsers.Contains(sessionUser.Id))
                    {
                        image.LikesCount = _dBService.GetImageVotesCount(image.Id, sessionId);
                        image.DislikesCount = _dBService.GetVotedUsersForImage(image.Id, sessionId).Count() - image.LikesCount;
                        notVoted.Add(image);
                    }
                }
                var voteViewModel = new VoteViewModel()
                {
                    Images = notVoted,
                    User = sessionUser                    
                };
                return View("UserVoting", voteViewModel);
            }
            
        }

        [HttpPost]
        public JsonResult AddVote(int imgId, int userId, int sessionId, string like)
        {
            try
            {
                _dBService.VoteImg(imgId, userId, sessionId, bool.Parse(like));
                return Json("success");
            }
            catch (Exception e)
            {
                return Json("error");
            }
            
        }
        

        public async Task<IActionResult> Index()
        {
            var user = GetCurrentUser();
            var images = 
            user.Images = _dBService.GetUserImages(user.Id).ToList();
            
            var roles = (await _manager.GetRolesAsync(user)).ToList();
            var userView = new UserViewModel()
            {
                Roles = roles,
                User = user
            };
            if (roles.Contains(Role.Teacher.ToString()))
            {
                return View("TeacherIndex", userView);
            }
            return View("UserIndex", userView);
        }

        private byte[] ConvertToByteArr(IFormFile formFile)
        {
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                return ms.ToArray();
            }
        }

        [HttpGet]
        public IActionResult Images()
        {
            var user = GetCurrentUser();
            var userViewModel = new UserViewModel()
            {
                User = user
            };

            return View(userViewModel);
        }

        private ApplicationUser GetCurrentUser()
        {
            return _manager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public JsonResult RemoveImage(string imgId)
        {
            var user = GetCurrentUser();
            _dBService.RemoveImage(int.Parse(imgId));
            return Json("success");
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<JsonResult> SaveImage()
        {
            try
            {
                foreach (var file in Request.Form.Files)
                {
                    if (file.Length > 0)
                    {
                        if (file.ContentType.Contains("image"))
                        {
                            using (MemoryStream m = new MemoryStream())
                            {
                                //var image = (Image)
                                //file. Save(m, file.RawFormat);
                                byte[] imageBytes = ConvertToByteArr(file);

                                // Convert byte[] to Base64 String
                                string base64String = Convert.ToBase64String(imageBytes);

                                var user = GetCurrentUser();

                                var image = new ImageModel() { Base64 = base64String };
                                
                                user.Images = _dBService.GetUserImages(user.Id).ToList();
                                if (user.Images == null)
                                {
                                    user.Images = new System.Collections.Generic.List<ImageModel>();
                                }

                                user.Images.Add(image);

                                var teacher = await _manager.FindByIdAsync(user.ParentUserId);
                                teacher.Images = _dBService.GetUserImages(teacher.Id).ToList();
                                if (teacher.Images == null)
                                {
                                    teacher.Images = new System.Collections.Generic.List<ImageModel>();
                                }

                                if (teacher.Images.Count >= 50)
                                {
                                    return Json("limit");
                                }

                                teacher.Images.Add(image);

                                var x = await _manager.UpdateAsync(user);
                                var y = await _manager.UpdateAsync(teacher);

                                return Json(new {image = image.Base64, imageId = image.Id });
                            }
                        }
                        
                    }
                }                
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");
        }

        

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
