using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTracker.Data;
using StudentTracker.Models;

namespace StudentTracker.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentTrackerContext _context;

        public StudentsController(StudentTrackerContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var studentTrackerContext = _context.Students.Include(s => s.AcademicDegree).Include(s => s.FormEducation).Include(s => s.Gender).Include(s => s.Specialty).Include(s => s.Specialty.Faculty);
            return View(await studentTrackerContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.AcademicDegree)
                .Include(s => s.FormEducation)
                .Include(s => s.Gender)
                .Include(s => s.Specialty)
                .FirstOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["AcademicDegreeID"] = new SelectList(_context.AcademicDegrees, "AcademicDegreeID", "AcademicDegreeID");
            ViewData["FormEducationID"] = new SelectList(_context.FormsEducation, "FormEducationID", "FormEducationID");
            ViewData["GenderID"] = new SelectList(_context.Genders, "GenderID", "GenderID");
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentID,LastName,FirstName,MidName,GenderID,PhoneNumber,EmailAddress,SpecialtyID,FormEducationID,AcademicDegreeID,IsHasRedDiploma,EndDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AcademicDegreeID"] = new SelectList(_context.AcademicDegrees, "AcademicDegreeID", "AcademicDegreeID", student.AcademicDegreeID);
            ViewData["FormEducationID"] = new SelectList(_context.FormsEducation, "FormEducationID", "FormEducationID", student.FormEducationID);
            ViewData["GenderID"] = new SelectList(_context.Genders, "GenderID", "GenderID", student.GenderID);
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", student.SpecialtyID);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["AcademicDegreeID"] = new SelectList(_context.AcademicDegrees, "AcademicDegreeID", "AcademicDegreeID", student.AcademicDegreeID);
            ViewData["FormEducationID"] = new SelectList(_context.FormsEducation, "FormEducationID", "FormEducationID", student.FormEducationID);
            ViewData["GenderID"] = new SelectList(_context.Genders, "GenderID", "GenderID", student.GenderID);
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", student.SpecialtyID);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentID,LastName,FirstName,MidName,GenderID,PhoneNumber,EmailAddress,SpecialtyID,FormEducationID,AcademicDegreeID,IsHasRedDiploma,EndDate")] Student student)
        {
            if (id != student.StudentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentID))
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
            ViewData["AcademicDegreeID"] = new SelectList(_context.AcademicDegrees, "AcademicDegreeID", "AcademicDegreeID", student.AcademicDegreeID);
            ViewData["FormEducationID"] = new SelectList(_context.FormsEducation, "FormEducationID", "FormEducationID", student.FormEducationID);
            ViewData["GenderID"] = new SelectList(_context.Genders, "GenderID", "GenderID", student.GenderID);
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", student.SpecialtyID);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.AcademicDegree)
                .Include(s => s.FormEducation)
                .Include(s => s.Gender)
                .Include(s => s.Specialty)
                .FirstOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }
    }
}
