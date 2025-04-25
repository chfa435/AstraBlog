using AstraBlog.Data;
using AstraBlog.Models;
using AstraBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace AstraBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAstraBlogService _blogPostService;
        private readonly IEmailSender _emailService;  
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context,
                              IAstraBlogService blogPostService,
                              UserManager<BlogUser> userManager,
                              IEmailSender emailSender)
        {
            _logger = logger;
            _blogPostService = blogPostService;
            _userManager = userManager;
            _context = context;
            _emailService = emailSender;
        }

        public async Task<IActionResult> Index(int? pageNum, string? swalMessage)
        {
            ViewData["SwalMessage"] = swalMessage;

            int pageSize = 3;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> model = (await _blogPostService.GetRecentPostsAsync()).ToPagedList(page, pageSize);

            return View(model);
        }

        public IActionResult SearchIndex(string? searchString, int? pageNum)
        {
            int pageSize = 5;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> model = (_blogPostService.SearchBlogPosts(searchString)).ToPagedList(page, pageSize);


            return View(nameof(Index), model);
        }

        public async Task<IActionResult> Popular(int? pageNum)
        {
            int pageSize = 4;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> model = (await _blogPostService.GetPopularPostsAsync()).ToPagedList(page, pageSize);

            return View(model);
        }

        public async Task<IActionResult> Recent(int? pageNum)
        {
            int pageSize = 4;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> model = (await _blogPostService.GetRecentPostsAsync()).ToPagedList(page, pageSize);

            return View(model);
        }

        public async Task<IActionResult> AllPosts(int? pageNum)
        {
            int pageSize = 6;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> model = (await _blogPostService.GetAllPostsAsync()).ToPagedList(page, pageSize);

            return View(model);
        }

        public async Task<IActionResult> ContactMe()
        {
            string? blogUserId = _userManager.GetUserId(User);
            BlogUser? blogUser = new(); 

            if (blogUserId != null) {
                blogUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == blogUserId);
            }

            return View(blogUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe([Bind("FirstName,LastName,Email")] BlogUser blogUser, string? message)
        {

            string? swalMessage = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                await _emailService.SendEmailAsync(blogUser.Email!, $"Contact Me Mesage From - {blogUser.FullName}", message!);
                    swalMessage = "Email sent successfully!";
                }catch(Exception) 
                {
                    swalMessage = "Error: Unable tosend email.";
                    throw;
                }
                
            }

            return RedirectToAction("Index", new { swalMessage });  
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}