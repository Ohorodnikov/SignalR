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
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;

namespace SignalR.Controllers
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
    }
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly UserManager<ApplicationUser> _manager;
        private readonly IDBService _dBService;
        private readonly IStringLocalizer<HomeController> _localizer;
        static HomeController()
        {
            _comments = new List<CommentModel>
            {
                new CommentModel
                {
                    Id = 1,
                    Author = "Daniel OlOlO Nigro",
                    Text = "Hello ReactJS.NET World!"
                },
                new CommentModel
                {
                    Id = 2,
                    Author = "Pete Hunt",
                    Text = "This is one comment"
                },
                new CommentModel
                {
                    Id = 3,
                    Author = "Jordan Walke",
                    Text = "This is *another* comment"
                },
            };
        }
        public HomeController(IHostingEnvironment appEnvironment, UserManager<ApplicationUser> manager, IDBService dBService, 
            IStringLocalizer<HomeController> localizer)
        {
            _appEnvironment = appEnvironment;
            _manager = manager;
            _dBService = dBService;
            _localizer = localizer;
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        private void AddLocalText()
        {
            ViewData["Contact"] = _localizer["Contact"];
            ViewData["Home"] = _localizer["Home"];
            ViewData["About"] = _localizer["About"];
        }

        [Route("comments/new")]
        [HttpPost]
        public ActionResult AddComment(CommentModel comment)
        {
            // Create a fake ID for this comment
            comment.Id = _comments.Count + 1;
            _comments.Add(comment);
            return Content("Success :)");
        }

        private static List<CommentModel> _comments;

        [Route("comments")]
        public IActionResult Comments()
        {
            //var sessions = _dBService.GetAllSessionUsers(code).ToList();
            //foreach (var s in sessions)
            //{
            //    s.Session = null;
            //}

           
            return Json(_comments);
        }

        public IActionResult Image(int id)
        {
            //AddLocalText();
            var img = _dBService.GetImage(id);
            return View(img);            
        }

        [HttpPost]
        public IActionResult Image(int id, string description)
        {
            try
            {
                var img = _dBService.GetImage(id);
                img.Description = description;
                _dBService.UpdateImage(img);
                return Json("success");
            }
            catch (Exception)
            {

                return Json("error");
            }
            
        }

        public IActionResult React(string code)
        {
            var sessions = _dBService.GetAllSessionUsers(code).ToList();
            foreach (var s in sessions)
            {
                s.Session = null;
            }
            return View();
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
                var likes = notVoted.Select(i => i.LikesCount).ToList();
                var x = likes.OrderByDescending(o => o);
                notVoted = notVoted.OrderByDescending(i => i.LikesCount).ToList();
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
            //AddLocalText();
            var user = GetCurrentUser();
            
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
            //AddLocalText();
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
        public JsonResult RemoveImage(int imgId)
        {
            _dBService.RemoveImage(imgId);
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
                                    user.Images = new List<ImageModel>();
                                }

                                user.Images.Add(image);

                                var teacher = await _manager.FindByIdAsync(user.ParentUserId);
                                teacher.Images = _dBService.GetUserImages(teacher.Id).ToList();
                                if (teacher.Images == null)
                                {
                                    teacher.Images = new List<ImageModel>();
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
            //AddLocalText();
            return View();
        }

        public IActionResult Contact()
        {                       
            ViewData["Message"] = _localizer["Contact"];
            //AddLocalText();

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
