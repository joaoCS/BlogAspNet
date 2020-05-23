using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAspNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlogAspNet.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> UsrMgr { get; }
        private SignInManager<AppUser> SignInMgr { get; }
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            UsrMgr = userManager;
            SignInMgr = signInManager;
            this.logger = logger;
        }

        public async Task<IActionResult> Logout()
        {
            await SignInMgr.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await SignInMgr.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            } 
            else
            {
                ViewBag.Result = "Result is " + result.ToString();
            }

            return View();
        }

        public async Task<IActionResult> Register()
        {
            try
            {
                ViewBag.Message = "Usuário já registrado!";
                AppUser user = await UsrMgr.FindByNameAsync("TestUser");

                if (user == null)
                {
                    user = new AppUser();

                    user.UserName = "TestUser";
                    user.Email = "TestUser@test.com";
                    user.FirstName = "John";
                    user.LastName = "Doe";

                    IdentityResult result = await UsrMgr.CreateAsync(user, "Test123!");
                    ViewBag.Message = "User was created";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UsrMgr.FindByEmailAsync(model.Email);

                if (user != null && await UsrMgr.IsEmailConfirmedAsync(user))
                {

                    var token = await UsrMgr.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    Console.WriteLine(passwordResetLink.ToString());

                    logger.Log(LogLevel.Warning, passwordResetLink);

                    return View("ForgotPasswordConfirmation");
                }

                ViewBag.Message = "Usuário não cadastrado ou email do usuário não foi confirmado!";
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Token de redefinição de senha inválido!");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            Console.WriteLine(model.Email);
            Console.WriteLine(model.Token);

            if (ModelState.IsValid)
            {
                var user = await UsrMgr.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await UsrMgr.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }


    }
}