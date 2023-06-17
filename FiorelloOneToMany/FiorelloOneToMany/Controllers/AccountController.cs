using FiorelloOneToMany.Models;
using FiorelloOneToMany.VıewModels.Account;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using FiorelloOneToMany.Services.Interfaces;
using FiorelloOneToMany.Helpers;

namespace FiorelloOneToMany.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailSevice;
        private readonly RoleManager<IdentityRole> _rolesManager;
        public AccountController(UserManager<AppUser> usermanager, SignInManager<AppUser> signInManager,IEmailService emailService,RoleManager<IdentityRole>roleManager)
        {
            _usermanager = usermanager;
            _signInManager = signInManager;
            _emailSevice = emailService;
            _rolesManager = roleManager;
        }

        [HttpGet]
        public  IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterVM request)
        {
            if(!ModelState.IsValid)
            {
                return View(request);
            }

            AppUser user = new()
            {
                UserName = request.UserName,
                Email = request.Email,
                FullName = request.FullName,
            };

            var result=await _usermanager.CreateAsync(user,request.Password);

            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,error.Description);
                }
                return View(request);
            }

            await _usermanager.AddToRoleAsync(user,Roles.SuperAdmin.ToString());


            string token =await _usermanager.GenerateEmailConfirmationTokenAsync(user);


            string link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id ,token},Request.Scheme);


            string html = string.Empty;

            using (StreamReader reader = new("wwwroot/templates/account.html"))
            {
                html = await reader.ReadToEndAsync();
            }

            html = html.Replace("{{link}}", link);

            html = html.Replace("{{fullName}}", user.FullName);

            string subject = "Email confirmation";
            _emailSevice.Send(user.Email, subject, html);



            return RedirectToAction(nameof(VerifyEmail));
        }

        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if (userId == null || token == null) return BadRequest();

            AppUser user=await _usermanager.FindByIdAsync(userId);

            await _usermanager.ConfirmEmailAsync(user, token);

            await _signInManager.SignInAsync(user, false);


            return RedirectToAction("Index", "Home");
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }


        [HttpGet]
        public IActionResult SignIn()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginVM request)
        {
            if(!ModelState.IsValid)
            {
                return View(request);
            }

            AppUser user = await _usermanager.FindByEmailAsync(request.EmailOrUsername);

            if(user == null)
            {
                user = await _usermanager.FindByNameAsync(request.EmailOrUsername);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or Password is wrong");
                return View(request);
            }

            PasswordVerificationResult comparePassword = _usermanager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (comparePassword.ToString()=="Failed")
            {
                ModelState.AddModelError(string.Empty, "Email or Password is wrong");
                return View(request);
            }


            var result = await _signInManager.PasswordSignInAsync(user,request.Password,false,false);

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Please confirm your account");
            }


            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

  
        public async Task<IActionResult> CreateRoles()
        {
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                if(await _rolesManager.RoleExistsAsync(role.ToString()))
                {
                    await _rolesManager.CreateAsync(new IdentityRole { Name=role.ToString()});
                }
            }
            return Ok();
        }
    }
}
