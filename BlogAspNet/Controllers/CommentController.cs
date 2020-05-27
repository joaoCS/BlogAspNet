using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAspNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogAspNet.Controllers
{
    public class CommentController : Controller
    {

        private UserManager<AppUser> _userManager;
        private readonly IdentityAppContext _context;

        public CommentController(IdentityAppContext context, UserManager<AppUser> userManager)
        {
            _context = context;

            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync(string texto, string id)
        {
            var user = await _userManager.GetUserAsync(User);

            Comment comm = new Comment();

            comm.AppUserFK = user.Id;
           
            comm.PostFK = Convert.ToInt32(id);
            comm.PostId = Convert.ToInt32(id);

            comm.Text = texto;

            _context.Add(comm);
            _context.SaveChanges();

            // Entrar na View Details com o Post sendo comentado
            return RedirectToAction("Details", "Post", new { id = comm.PostFK});       
        }

    }
}