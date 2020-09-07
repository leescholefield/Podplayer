using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Podplayer.ASP.Models;
using Podplayer.Entity.Identity;
using System.Threading.Tasks;

namespace Podplayer.ASP.Controllers
{
    /// <summary>
    /// Responsible for actions relating to user accounts: signing in/out, registering new accounts, and, in the future, altering account 
    /// details.
    /// </summary>
    public class AccountController : BaseController
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager, 
                                ILogger<AccountController> logger, 
                                IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Retrieves the 'Register' view for registing new accounts.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (ReturnAsPartialView())
                return PartialView();

            return View("Register");
        }

        /// <summary>
        /// Recieves a <see cref="SignInViewModel"/> containing the neccessary details to create an account; at a minimum this is a user name 
        /// and a password. If the account was successfully created this will redirect to <paramref name="returnUrl"/>. If it could not be 
        /// created it will return to the 'register' page with a message explaining why.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterPost(SignInViewModel viewModel, string returnUrl)
        {
            if (ViewModelIsValid(viewModel))
            {
                var user = new AppUser { UserName = viewModel.Username };
                var result = await _userManager.CreateAsync(user, viewModel.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("New account registered with username {0}", user.UserName);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl ?? "/");
                }
            }

            // If we got this far, something failed, redisplay form
            return View("Register");
        }

        /// <summary>
        /// Retrieves the SignIn razor view.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            if (ReturnAsPartialView())
                return PartialView("SignIn");
            return View("SignIn");
        }

        /// <summary>
        /// Attempts to sign the user in. If this is successfull it will then redirect to <paramref name="returnUrl"/>, or the home page if 
        /// no returnUrl is supplied. If it is not successful then the user will be redirected back to the 'SignIn' view.
        /// </summary>
        /// <param name="viewModel">Contains sign in details.</param>
        /// <param name="returnUrl">Optional url to redirect the user to on success; if null then the home page will be returned.</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SignIn")]
        public async Task<IActionResult> SignInPost(SignInViewModel viewModel, string returnUrl)
        {
            if (ViewModelIsValid(viewModel))
            {
                var result = await _signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password, viewModel.RememberMe, false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl ?? "/");
                }
            }

            // If we got this far something failed, redisplay form
            return View("SignIn");
        }

        [HttpPost]
        public async Task<IActionResult> SignOut(string returnuUrl)
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect(returnuUrl ?? "/");
        }

        /// <summary>
        /// Returns '_LoginPartial' as a standalone view.
        /// 
        /// This is used when the user logs in or out and the client doesn't want to refresh the page or redirect to another. The result of 
        /// this call is then swaped with the current views login view. 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> LoginPartialView()
        {
            return PartialView("_LoginPartial");
        }

        private bool ViewModelIsValid(SignInViewModel vm)
        {
            bool hasUsername = vm.Username != null && vm.Username != "";
            bool hasPassword = vm.Password != null;
            return hasUsername && hasPassword;
        }
    }
}
