// Necessary namespaces for the controller's functionality, including models, MVC features, authorization, and database context.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScavengeRUs.Data;
using ScavengeRUs.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using ScavengeRUs.Services;

namespace ScavengeRUs.Controllers
{
    // The controller is protected with an authorization policy ensuring only users with the Admin role can access most of its actions.
    public class LocationsController : Controller
    {
        // Dependencies injected into the controller include repositories for users and hunts, and the application's database context.
        private readonly IUserRepository _userRepo;
        private readonly IHuntRepository _huntRepo;
        private readonly ApplicationDbContext _context;

        // Constructor injects the dependencies.
        public LocationsController(ApplicationDbContext context, IHuntRepository huntRepo, IUserRepository userRepo)
        {
            _userRepo = userRepo;
            _huntRepo = huntRepo;
            _context = context;
        }

        // Displays a list of all locations. This action requires Admin role authorization.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Location.ToListAsync());
        }

        // Shows the details of a specific location identified by its Id. Requires Admin authorization.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Location == null)
                return NotFound();

            var location = await _context.Location.FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
                return NotFound();

            return View(location);
        }

        // Presents a form to create a new location. Requires Admin authorization.
        [Authorize(Roles = "Admin")]
        public IActionResult Create([Bind(Prefix = "Id")] int huntid)
        {
            return View();
        }

        // Processes the submission of a new location. If linked to a hunt, redirects to manage tasks for that hunt; otherwise, returns to the index. Requires Admin authorization.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind(Prefix = "Id")] int huntid, Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Add(location);
                await _context.SaveChangesAsync();
                if (huntid != 0)
                    return RedirectToAction("ManageTasks", "Hunt", new { id = huntid });

                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // Renders an edit form for a specific location. Requires Admin authorization.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Location == null)
                return NotFound();

            var location = await _context.Location.FindAsync(id);
            if (location == null)
                return NotFound();

            return View(location);
        }

        // Handles the submission of edits to a location. Ensures data consistency and handles concurrency. Requires Admin authorization.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HuntId,Place,Lat,Lon,Task,AccessCode,QRCode,Answer")] Location location)
        {
            if (id != location.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // Shows a confirmation page for deleting a location. Requires Admin authorization.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Location == null)
                return NotFound();

            var location = await _context.Location.FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
                return NotFound();

            return View(location);
        }

        // Confirms the deletion of a location and removes it from the database. Requires Admin authorization.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Location == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Location'  is null.");
            }
            var location = await _context.Location.FindAsync(id);
            if (location != null)
            {
                _context.Location.Remove(location);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Validates whether a specific location exists in the database.
        private bool LocationExists(int id)
        {
            return _context.Location.Any(e => e.Id == id);
        }

        // Validates a player's answer to a task associated with a location. This method supports both Admin and Player roles and is designed for AJAX calls.
        [HttpPost]
        [Authorize(Roles = "Admin, Player")]
        public async Task<IActionResult> ValidateAnswer([FromForm]int id, int taskid, string answer)
        {
            var currentUser = await _userRepo.ReadAsync(User.Identity?.Name!);
            var location = await _context.Location.FirstOrDefaultAsync(m => m.Id == taskid);
            if (answer != null && answer.Equals(location?.Answer, StringComparison.OrdinalIgnoreCase))
            {
                currentUser?.TasksCompleted!.Add(location); // Update the player's completed tasks
                await _context.SaveChangesAsync();
                return Json(new { success = true});
            }
            return Json(new { success = false });
        }
    }
}

