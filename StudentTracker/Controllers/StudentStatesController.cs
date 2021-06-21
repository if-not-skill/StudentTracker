using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTracker.Data;
using StudentTracker.Models;

namespace StudentTracker.Controllers
{
    [Authorize(Roles = "admin, worker")]
    public class StudentStatesController : Controller
    {
        private readonly StudentTrackerContext _context;

        public StudentStatesController(StudentTrackerContext context)
        {
            _context = context;
        }

        // GET: StudentStates
        public async Task<IActionResult> Index()
        {
            var studentTrackerContext = _context.StudentStates.Include(s => s.EmploymentStatus).Include(s => s.Student);
            return View(await studentTrackerContext.ToListAsync());
        }

        // GET: StudentStates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentState = await _context.StudentStates
                .Include(s => s.EmploymentStatus)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.StudentStateID == id);
            if (studentState == null)
            {
                return NotFound();
            }

            return View(studentState);
        }

        // GET: StudentStates/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["StudentID"] = id;
            ViewData["EmploymentStatusID"] = new SelectList(_context.EmploymentStatuses, "EmploymentStatusID", "Name");
            ViewData["Student"] = _context.Students.Find(id);
            return View();
        }

        // POST: StudentStates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentStateID,StudentID,EmploymentStatusID,StatusDate,CountryName,CityName,OrganizationName,PostName")] StudentState studentState)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentState);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Students", new { id = studentState.StudentID });
            }
            ViewData["EmploymentStatusID"] = new SelectList(_context.EmploymentStatuses, "EmploymentStatusID", "EmploymentStatusID", studentState.EmploymentStatusID);
            ViewData["StudentID"] = new SelectList(_context.Students, "StudentID", "EmailAddress", studentState.StudentID);
            return View();
        }

        // GET: StudentStates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentState = await _context.StudentStates.FindAsync(id);
            if (studentState == null)
            {
                return NotFound();
            }
            ViewData["EmploymentStatusID"] = new SelectList(_context.EmploymentStatuses, "EmploymentStatusID", "Name", studentState.EmploymentStatusID);
            ViewData["StudentID"] = new SelectList(_context.Students, "StudentID", "LastName", studentState.StudentID);
            return View(studentState);
        }

        // POST: StudentStates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentStateID,StudentID,EmploymentStatusID,StatusDate,CountryName,CityName,OrganizationName,PostName")] StudentState studentState)
        {
            if (id != studentState.StudentStateID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentState);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentStateExists(studentState.StudentStateID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Students", new{id = studentState.StudentID});
            }
            ViewData["EmploymentStatusID"] = new SelectList(_context.EmploymentStatuses, "EmploymentStatusID", "EmploymentStatusID", studentState.EmploymentStatusID);
            ViewData["StudentID"] = new SelectList(_context.Students, "StudentID", "EmailAddress", studentState.StudentID);
            return View(studentState);
        }

        // GET: StudentStates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentState = await _context.StudentStates
                .Include(s => s.EmploymentStatus)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.StudentStateID == id);
            if (studentState == null)
            {
                return NotFound();
            }

            return View(studentState);
        }

        // POST: StudentStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentState = await _context.StudentStates.FindAsync(id);
            _context.StudentStates.Remove(studentState);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Students", new {id = studentState.StudentID});
        }

        private bool StudentStateExists(int id)
        {
            return _context.StudentStates.Any(e => e.StudentStateID == id);
        }
    }
}
