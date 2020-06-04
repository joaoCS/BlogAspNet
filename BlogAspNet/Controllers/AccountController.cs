using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAspNet.Models;
using BlogAspNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration;

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

            return RedirectToAction("Index", "Post");
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
                return RedirectToAction("Index", "Post");
            } 
            else
            {
                ViewBag.Result = "Result is " + result.ToString();
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(AppUser user, string password)
        {
            if (ModelState.IsValid)
            {
                try
                {
                  
                    var result = await UsrMgr.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        var token = await UsrMgr.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, token = token }, Request.Scheme);

                        logger.Log(LogLevel.Warning, confirmationLink);


                        string emailHtmlBody = "<!DOCTYPE html>" +
                        "<html>" +
                        "<head>" +
                        "<title></title>" +
                        "</head>" +
                        "<body>" +
                        "Clique no link para confirmar seu email:" +
                        "<br/>" +
                        "<a href='" + confirmationLink.ToString() + "'>Confirmar email</a>" +
                        "</body>" +
                        "</html>";

                        SendEmail.Send(user.Email, "BlogAspNet - Confirmar email", emailHtmlBody);


                        ViewBag.MessageTitle = "Registro feito com sucesso!";
                        ViewBag.MessageBody = "Antes que você possa fazer login, por favor, confirme " +
                                "seu email clicando no link de confirmação que mandamos para seu email";
                        return View("ConfirmEmail");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);

                    return View();
                }
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Post");
            }

            var user = await UsrMgr.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"O Id de usuário {userId} é inválido";
                return View("Error");
            }

            var result = await UsrMgr.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewBag.Message = "Email confirmado com sucesso!";

                return View("Messages");
            }

            ViewBag.ErrorMessage = "Email não pôde ser confirmado :(";
            return View("Error");
        }


        //[HttpPost]
        //public async Task<IActionResult> Register(AppUser user, string password)
        //{
        //    try
        //    {
        //        AppUser usr = await UsrMgr.FindByEmailAsync(user.Email);

        //        if (usr == null)
        //        {
        //            //user.UserName = "TestUser";
        //            //user.Email = "TestUser@test.com";
        //            //user.FirstName = "John";
        //            //user.LastName = "Doe";

        //            IdentityResult result = await UsrMgr.CreateAsync(user, password);

        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error.Description);
        //            }

        //            if (result.Succeeded) { 
        //                ViewBag.Message = "Usuário foi registrado";

        //                return View("RegisterConfirmation");
        //            }

        //            return View();
        //        }

        //        ViewBag.Message = "Email já registrado!";
        //        return View();

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;

        //        return View();
        //    }
        //}

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

                    logger.Log(LogLevel.Warning, passwordResetLink);

                    string emailHtmlBody = "<!DOCTYPE html>" +
                        "<html>" +
                        "<head>" +
                        "<title></title>" +
                        "</head>" +
                        "<body>" +
                        "Clique no link para redefinir sua senha:" +
                        "<br/>" +
                        "<a href='" + passwordResetLink.ToString() + "'>Redefinir senha</a>" +
                        "</body>" +
                        "</html>";

                    SendEmail.Send(model.Email, "BlogAspNet - Redefinir senha", emailHtmlBody);

                    return View("ForgotPasswordConfirmation");
                }

                ViewBag.Message = "Usuário não cadastrado ou email do usuário não foi confirmado!";
                return View();
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

                ModelState.AddModelError("", "Usuário não encontrado!");
                
                return View(model);
            }

            ModelState.AddModelError("", "Você forneceu algum dado inválido");

            return View(model);
        }
    }
}