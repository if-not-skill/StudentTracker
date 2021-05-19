using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTracker.Data;
using StudentTracker.Models;

namespace StudentTracker.Controllers
{
    public class SpecialtiesController : Controller
    {
        private readonly StudentTrackerContext _context;

        public SpecialtiesController(StudentTrackerContext context)
        {
            _context = context;
        }

        // GET: Specialties
        public async Task<IActionResult> Index()
        {
            var studentTrackerContext = _context.Specialties.Include(s => s.Faculty);
            return View(await studentTrackerContext.ToListAsync());
        }

        // GET: Specialties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .Include(s => s.Faculty)
                .FirstOrDefaultAsync(m => m.SpecialtyID == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // GET: Specialties/Create
        public IActionResult Create(int id = 1)
        {
            ViewData["FacultyID"] = new SelectList(_context.Faculties, "FacultyID", "FacultyName", id);
            return View();
        }

        // POST: Specialties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SpecialtyID,SpecialtyName,FacultyID")] Specialty specialty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialty);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Faculties");
            }
            ViewData["FacultyID"] = new SelectList(_context.Faculties, "FacultyID", "FacultyID", specialty.FacultyID);
            return View(specialty);
        }

        // GET: Specialties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }
            ViewData["FacultyID"] = new SelectList(_context.Faculties, "FacultyID", "FacultyName", specialty.FacultyID);
            return View(specialty);
        }

        // POST: Specialties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SpecialtyID,SpecialtyName,FacultyID")] Specialty specialty)
        {
            if (id != specialty.SpecialtyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(specialty.SpecialtyID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Faculties");
            }
            ViewData["FacultyID"] = new SelectList(_context.Faculties, "FacultyID", "FacultyID", specialty.FacultyID);
            return View(specialty);
        }

        // GET: Specialties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .Include(s => s.Faculty)
                .FirstOrDefaultAsync(m => m.SpecialtyID == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Faculties");
        }

        private bool SpecialtyExists(int id)
        {
            return _context.Specialties.Any(e => e.SpecialtyID == id);
        }
    }
}
