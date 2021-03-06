﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlogAspNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BlogAspNet.Controllers
{
    public class PostController : Controller
    {
        private readonly IdentityAppContext _context;

        private UserManager<AppUser> _userManager;

        public PostController(IdentityAppContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = user;
            return View(_context.Posts.ToList());
        }

        [Authorize]
        public IActionResult CreatePost()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePostAsync(Post post)
        {

            var user = await _userManager.GetUserAsync(User);

            post.AppUserFK = user.Id;
            post.UserName = user.FirstName + " " + user.LastName;

            _context.Add(post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Editar(int id)
        {
            var post = _context.Posts.Find(id);
            return View(post);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Editar(Post post)
        {
            _context.Update(post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Excluir (int id)
        {
            var post = _context.Posts.Find(id);
            return View(post);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Apagar(int id)
        {
            var post = _context.Posts.Find(id);
            _context.Remove(post);
            _context.SaveChanges();
           
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DetailsAsync(int id)
        {
            var post = _context.Posts.Find(id);

            var comments = _context.Comments.Where(c => c.PostId == id).ToList();

            var commentsListWithOwners = Enumerable.Repeat(new CommentClass() { Id = 0, Nome = "", Texto = "", OwnerId = 0 }, 0).ToList();
            //list.Add(new { ID = 753159, Name = "Lamont Cranston" });

            foreach (var comment in comments)
            {
                //pega o dono do comentário
                var usr = _userManager.Users.Where(user => user.Id == comment.AppUserFK).First();

                commentsListWithOwners.Add(new CommentClass() {Id = comment.Id, Nome = usr.FirstName + " " + usr.LastName, Texto = comment.Text, OwnerId = comment.AppUserFK});
            }

            var user = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = user;

            ViewBag.Comments = commentsListWithOwners;

            return View(post);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
