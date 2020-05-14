using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlogAspNet.Models;

namespace BlogAspNet.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;

        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Posts.ToList());
        }

        public IActionResult CreatePost()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult CreatePost(Post post)
        {

            _context.Add(post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            var post = _context.Posts.Find(id);
            return View(post);
        }

        [HttpPost]
        public IActionResult Editar(Post post)
        {
            _context.Update(post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Excluir (int id)
        {
            var post = _context.Posts.Find(id);
            return View(post);
        }

        [HttpPost]
        public IActionResult Apagar(int id)
        {
            var post = _context.Posts.Find(id);
            _context.Remove(post);
            _context.SaveChanges();
           
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var post = _context.Posts.Find(id);
            
            return View(post);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
