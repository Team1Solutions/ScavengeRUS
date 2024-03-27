using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengeRUs.Models.Entities;
using ScavengeRUs.Services;
using Microsoft.AspNetCore.Identity;
using ScavengeRUs.Attributes;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ScavengeRUs.Controllers
{
    /// <summary>
    /// This class is the controller for any page realted to hunts
    /// </summary>
    public class HuntController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IHuntRepository _huntRepo;

        /// <summary>
        /// Injecting the user repository and hunt repository (Db classes)
        /// </summary>
        /// <param name="userRepo"></param>
        /// <param name="HuntRepo"></param>
        public HuntController(IUserRepository userRepo, IHuntRepository HuntRepo)
        {
            _userRepo = userRepo;
            _huntRepo = HuntRepo;
        }
        
        
        /// <summary>
        /// www.localhost.com/hunt/index Returns a list of all hunts
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string sortOrder)
        {
            //Parameters sent from view determine sort order of the hunts
            ViewBag.CreationDateSortParm = sortOrder == "creation_date" ? "creation_date_desc" : "creation_date";
            ViewBag.StartDateSortParm = sortOrder == "start_date" ? "start_date_desc" : "start_date";
            ViewBag.HuntNameSortParm = sortOrder == "hunt_name" ? "hunt_name_desc" : "hunt_name";
            ViewBag.StatusSortParm = sortOrder == "status" ? "status_desc" : "status";
            ViewBag.EndDateSortParm = sortOrder == "end_date" ? "end_date_desc" : "end_date";
            ViewBag.PlayerSortParm = sortOrder == "player" ? "player_desc" : "player";
            ViewBag.TasksSortParm = sortOrder == "tasks" ? "tasks_desc" : "tasks";

            var hunts = await _huntRepo.ReadAllAsync();

            //Sorting functionality for columns on view
            //There is probably a better way to refactor this sorting functionality, this method was shown in the Microsoft Docs
            switch(sortOrder)
            {
                case "creation_date":
                    hunts = hunts.OrderBy(h => h.CreationDate).ToList();
                    break;
                case "creation_date_desc":
                    hunts = hunts.OrderByDescending(h => h.CreationDate).ToList();
                    break;
                case "start_date":
                    hunts = hunts.OrderBy(h => h.StartDate).ToList();
                    break;
                case "start_date_desc":
                    hunts = hunts.OrderByDescending(h => h.StartDate).ToList();
                    break;
                case "hunt_name":
                    hunts = hunts.OrderBy(h => h.HuntName).ToList();
                    break;
                case "hunt_name_desc":
                    hunts = hunts.OrderByDescending(h => h.HuntName).ToList();
                    break;
                case "status":
                    //Sorts by whether the hunt has ended or not
                    hunts = hunts.OrderBy(h => TimeSpan.Parse((h.EndDate - DateTime.Now).ToString()).Seconds < 0).ToList();
                    break;
                case "status_desc":
                    hunts = hunts.OrderByDescending(h => TimeSpan.Parse((h.EndDate - DateTime.Now).ToString()).Seconds < 0).ToList();
                    break;
                case "end_date":
                    hunts = hunts.OrderBy(h => h.EndDate).ToList();
                    break;
                case "end_date_desc":
                    hunts = hunts.OrderByDescending(h => h.EndDate).ToList();
                    break;
                case "player":
                    hunts = hunts.OrderBy(h => h.Players.Count).ToList();
                    break;
                case "player_desc":
                    hunts = hunts.OrderByDescending(h => h.Players.Count).ToList();
                    break;
                case "tasks":
                    hunts = hunts.OrderBy(h => h.HuntLocations.Count).ToList();
                    break;                
                case "tasks_desc":
                    hunts = hunts.OrderByDescending(h => h.HuntLocations.Count).ToList();
                    break;
                default:
                    hunts = hunts.OrderBy(h => h.Id).ToList();
                    break;
            }

            return View(hunts);
        }
        
        
        /// <summary>
        /// www.localhost.com/hunt/create This is the get method for creating a hunt
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// www.localhost.com/hunt/create This is the post method for creating a hunt
        /// </summary>
        /// <param name="hunt"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Hunt hunt)
        {
            if (ModelState.IsValid)
            {
                hunt.CreationDate = DateTime.Now;
                await _huntRepo.CreateAsync(hunt);
                return RedirectToAction("Index");
            }
            return View(hunt);
           
        }
        /// <summary>
        /// www.localhost.com/hunt/details/{huntId} This is the details view of a hunt
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details([Bind(Prefix ="Id")]int huntId)
        {
            if (huntId == 0)
                return RedirectToAction("Index");

            var hunt = await _huntRepo.ReadAsync(huntId);
            if (hunt == null)
                return RedirectToAction("Index");

            return View(hunt);
        }
        /// <summary>
        /// www.localhost.com/hunt/delete/{huntId} This is the get method for deleting a hunt
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([Bind(Prefix = "Id")]int huntId)
        {
            if (huntId == 0)
                return RedirectToAction("Index");

            var hunt = await _huntRepo.ReadAsync(huntId);
            if (hunt == null)
                return RedirectToAction("Index");

            return View(hunt);
        }


        /// <summary>
        /// www.localhost.com/hunt/delete/{huntId} This is the post method for deleteing a hunt.
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([Bind(Prefix = "id")] int huntId)
        {
            await _huntRepo.DeleteAsync(huntId);
            return RedirectToAction("Index");
        }


        /// <summary>
        /// www.localhost.com/hunt/viewplayers/{huntId} Returns a list of all players in a specified hunt
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewPlayers([Bind(Prefix = "Id")] int huntId)
        {
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntId);
            ViewData["Hunt"] = hunt;
            if(hunt == null)
                return RedirectToAction("Index");
            
            return View(hunt.Players);
        }
        
        
        /// <summary>
        /// www.localhost.com/hunt/addplayertohunt{huntid} Get method for adding a player to a hunt. 
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPlayerToHunt([Bind(Prefix ="Id")]int huntId)
        {
            var hunt = await _huntRepo.ReadAsync(huntId);
            ViewData["Hunt"] = hunt;
            return View();
            
        }
        
        
        /// <summary>
        /// www.localhost.com/hunt/addplayertohunt{huntid} Post method for the form submission. This creates a user and assigns the access code for the hunt. 
        /// </summary>
        /// <param name="huntId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddPlayerToHunt([Bind(Prefix = "Id")] int huntId ,ApplicationUser user)
        {

            if (huntId == 0)
            {
                RedirectToAction("Index");
            }
            var hunt = await _huntRepo.ReadAsync(huntId);
            var existingUser = await _userRepo.ReadAsync(user.Email);
            var newUser = new ApplicationUser();
            if (existingUser == null)
            {
                newUser.Email = user.Email;
                newUser.PhoneNumber = user.PhoneNumber;
                newUser.FirstName = user.FirstName;
                newUser.LastName = user.LastName;
                newUser.AccessCode = user.AccessCode;
                newUser.UserName = user.Email;
            }
            else
            {
                newUser = existingUser;
                newUser.AccessCode = user.AccessCode;
            }
            if (newUser.AccessCode!.Code == null)       //If the admin didn't specify an access code (If we need to, I have the field readonly currently)
            {
                newUser.AccessCode = new AccessCode()
                {
                    Hunt = hunt,                        //Setting foriegn key
                    Code = $"{newUser.PhoneNumber}/{hunt.HuntName!.Replace(" ", string.Empty)}",            //This is the access code generation
                };
                newUser.AccessCode.Users.Add(newUser);  //Setting foriegn key
            }
            else
            {
                newUser.AccessCode = new AccessCode()
                {
                    Hunt = hunt,
                    Code = newUser.AccessCode.Code,
                };
                newUser.AccessCode.Users.Add(newUser);
            }
            
            //Set default value for email body
            string emailBody = $"<div>Hi {newUser.FirstName} {newUser.LastName} welcome to the ETSU Scavenger Hunt game! " +
                   $"To get started please go to the BucHunt website and login with the access code: {newUser.AccessCode.Code}</div>";
            
            if(hunt.InvitationBodyText is not null)
            {
                var userStr = hunt.InvitationBodyText.Replace("%user", $"{newUser.FirstName} {newUser.LastName}");
                emailBody = userStr.Replace("%code", $"{newUser.AccessCode.Code}");
            }
            await _huntRepo.AddUserToHunt(huntId, newUser); //This methods adds the user to the database and adds the database relationship to a hunt.

            string subject = hunt.InvitationText ?? "Welcome to the ETSU Scavenger Hunt!";

			await Functions.SendEmail(newUser.Email, subject, emailBody);

            //Nick Sells, 11/29/2023: get this value from the user, instead of just hardcoding in verizon
            //we have hard coded in verizon because thats what we all have
            newUser.Carrier = Models.Enums.Carrier.Verizon;
            await Functions.SendSMS(newUser.Carrier, newUser.PhoneNumber, $"{subject}\n{emailBody}");
            return RedirectToAction("Index");
        }


        /// <summary>
        /// www.localhost.com/hunt/removeuser/{username}/{huntId} This is the get method for removing a user from a hunt.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUser([Bind(Prefix ="Id")]string username, [Bind(Prefix ="huntId")]int huntid)
        {
            ViewData["Hunt"] = huntid;
            var user = await _userRepo.ReadAsync(username);
            return View(user);
        }
        
        
        /// <summary>
        /// www.localhost.com/hunt/removeuser/{username}/{huntId} This is the post method for removing a user from a hunt.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RemoveUserConfirmed(string username, int huntid)
        {
            await _huntRepo.RemoveUserFromHunt(username, huntid);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// This method generates a view of all task associated with a hunt. Pasing the huntid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Player, Admin")]
        public async Task<IActionResult> ViewTasks([Bind(Prefix ="Id")]int huntid)
        {
            var currentUser = await _userRepo.ReadAsync(User.Identity?.Name!);
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntid);
            ViewData["Hunt"] = hunt;
            ViewData["CurrentUser"] = currentUser;
            if (hunt == null)
                return RedirectToAction("Index");            

            var tasks = await _huntRepo.GetLocations(hunt.HuntLocations);
            return View(tasks.OrderBy(o => currentUser?.TasksCompleted?.Contains(o)));
            
        }


        /// <summary>
        /// This method shows all tasks that can be added to the hunt. Exculding the tasks that are already added
        /// </summary>
        /// <param name="huntid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageTasks([Bind(Prefix ="Id")]int huntid)
        {
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntid);
            //var existingLocations = await _huntRepo.GetLocations(hunt.HuntLocations);

            ViewData["Hunt"] = hunt;
            var allLocations = await _huntRepo.GetAllLocations();
            //var locations = allLocations.Except(existingLocations);
            return View(allLocations);
        }


        /// <summary>
        /// This method is the post method for adding a task. This gets executed when you click "Add Task"
        /// </summary>
        /// <param name="id"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTask(int id, int huntid)
        {
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntid);
            ViewData["Hunt"] = hunt;
            await _huntRepo.AddLocation(id, huntid);
            return RedirectToAction("ManageTasks", new {id=huntid});
        }


        /// <summary>
        /// This is the get method for removing a task from a hunt. This is executed when clicking "Remove" from the Hunt/ViewTasks screen
        /// </summary>
        /// <param name="id"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveTasks(int id, int huntid)
        {
            var hunt = await _huntRepo.ReadAsync(huntid);
            ViewData["Hunt"] = hunt;
            var task = await _huntRepo.ReadLocation(id);
            return View(task);
        }


        /// <summary>
        /// This is the post method for removing a task. This is executed when you click "Remove" from the Hunt/RemoveTask screen
        /// </summary>
        /// <param name="id"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveTask(int id, int huntid)
        {
            await _huntRepo.RemoveTaskFromHunt(id, huntid);
            return RedirectToAction("ManageTasks", "Hunt", new {id=huntid});
        }


        /// <summary>
        /// updates all hunts
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var hunt = await _huntRepo.ReadAsync(id);
            return View(hunt);
        }

        /// <summary>
        /// update operation for hunts
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hunt"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [EndDateDateValidation(ErrorMessage = "End date must be equal to or after the start date.")]    // does not work as of now
        public IActionResult Update(int id, Hunt hunt)
        {
            if (hunt.EndDate < hunt.StartDate)
                ModelState.AddModelError("EndDate", "End date must be equal to or after the start date.");

            if (ModelState.IsValid)
            {
                _huntRepo.Update(id, hunt);
                return RedirectToAction("Index");
            }

            return View(hunt);
        }
    }
}
