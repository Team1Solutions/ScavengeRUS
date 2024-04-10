using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScavengeRUs.Models;
using ScavengeRUs.Models.Entities;
using ScavengeRUs.Services;
using System.Diagnostics;

namespace ScavengeRUs.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<HomeController> _logger;

        
        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        public HomeController(ILogger<HomeController> logger, IUserRepository userRepo, SignInManager<ApplicationUser> signInRepo)
        {
            _signInRepo = signInRepo;
            _userRepo = userRepo;
            _logger = logger; 
        }


        /// <summary>
        /// Landing page for localhost
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View(); //Right click and go to view to see the HTML or see it in the Views/Home folder in the solution explorer
        }


        /// <summary>
        /// Landing page for login page
        /// </summary>
        /// <returns></returns>
        public IActionResult LogIn()
        {
            return View();
        }
        
        
        /// <summary>
        /// Landing page for after a user logs in
        /// </summary>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        [HttpPost, ActionName("LogIn")]
        public async Task<IActionResult> LogInConfirmed(AccessCode accessCode)
        {
            // Checks to see if access code is null and if it is tells user to enter one
            Guard.IsNotNull(accessCode);

            var user = await _userRepo.FindByAccessCode(accessCode.Code!);
            if (user == null)
            {
                // Add an error message to the ViewBag or ModelState
                ViewBag.ErrorMessage = "Invalid Access Code!";
                // Return to the login view, potentially passing back the access code for user convenience
                return View("Login", accessCode);
            }
            // If access code is correct signs in user and redirects to hunt page
            await _signInRepo.SignInAsync(user, false);
            return RedirectToAction("ViewTasks", "Hunt", new { id = user.Hunt.Id });
        }



        /// <summary>
        /// Landing page for privacy policy
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }


        /// <summary>
        /// This is the page displayed if there were a error
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}