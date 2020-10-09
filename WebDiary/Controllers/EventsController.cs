using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebDiary.Authorization;
using WebDiary.Data;
using WebDiary.Models;

namespace WebDiary.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<IdentityUser> _userManager;


        public EventsController(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchString, int page = 0)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult();
            }

            var pageSize = 8;
            var totalPosts = _context.Event.Count();
            var totalPages = totalPosts / pageSize;
            var previousPage = page - 1;
            var nextPage = page + 1;

            ViewBag.PreviousPage = previousPage;
            ViewBag.HasPreviousPage = previousPage >= 0;
            ViewBag.NextPage = nextPage;
            ViewBag.HasNextPage = nextPage < totalPages;

            var eventi = from e in _context.Event
                         select e;

            bool isAuthorized = User.IsInRole(Constants.EventAdministratorsRole);
            if (!isAuthorized)
            {
                string currentUserId = _userManager.GetUserId(User);
                eventi = eventi.Where(e => e.created_user_id == currentUserId);
            }

            // string search in diary_text        
            if (!String.IsNullOrEmpty(searchString))
            {
                eventi = eventi.Where(e => e.event_text.Contains(searchString));
            }

            // paging filter
            eventi = eventi.OrderByDescending(e => e.event_date)
                                .Skip(pageSize * page)
                                .Take(pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView(eventi.ToArray());

            return View(await eventi.AsNoTracking().ToListAsync());

        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult();
            }

            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.id == id);

            if (@event == null)
            {
                return NotFound();
            }

            bool isAuthorized = User.IsInRole(Constants.EventAdministratorsRole);
            string currentUserId = _userManager.GetUserId(User);
            if (!isAuthorized
                && currentUserId != @event.created_user_id)
            {
                return Forbid();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,event_text,event_date,created_user_id,created_on,modified_user_id,modified_on")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult();
            }

            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            bool isAuthorized = User.IsInRole(Constants.EventAdministratorsRole);
            string currentUserId = _userManager.GetUserId(User);
            if (!isAuthorized
                && currentUserId != @event.created_user_id)
            {
                return Forbid();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,event_text,event_date,created_user_id,created_on,modified_user_id,modified_on")] Event @event)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult();
            }

            if (id != @event.id)
            {
                return NotFound();
            }

            bool isAuthorized = User.IsInRole(Constants.EventAdministratorsRole);
            string currentUserId = _userManager.GetUserId(User);
            if (!isAuthorized
                && currentUserId != @event.created_user_id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);

        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult();
            }

            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.id == id);

            bool isAuthorized = User.IsInRole(Constants.EventAdministratorsRole);
            string currentUserId = _userManager.GetUserId(User);
            if (!isAuthorized
                && currentUserId != @event.created_user_id)
            {
                return Forbid();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult();
            }

            var @event = await _context.Event.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            bool isAuthorized = User.IsInRole(Constants.EventAdministratorsRole);
            string currentUserId = _userManager.GetUserId(User);
            if (!isAuthorized
                && currentUserId != @event.created_user_id)
            {
                return Forbid();
            }

            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.id == id);
        }
    }
}
