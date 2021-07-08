using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTracker.Data;
using StudentTracker.Models;

namespace StudentTracker.Controllers
{
    public class ReportsController : Controller
    {
        private readonly StudentTrackerContext _context;

        public ReportsController(StudentTrackerContext context)
        {
            _context = context;
        }

        // GET: Reports
        public async Task<IActionResult> Index(int faculty = 0, int specialty = 0, int formEducation = 0, int academicDegree = 0, int sex = 0, int endYear = 0)
        {
            ViewData["FacultyId"] = faculty;
            ViewData["SpecialtyId"] = specialty;
            ViewData["FormEducationId"] = formEducation;
            ViewData["AcademicDegreeId"] = academicDegree;
            ViewData["Sex"] = sex;
            ViewData["EndYear"] = endYear;

            var students = _context.Students
                .Include(s => s.AcademicDegree)
                .Include(s => s.FormEducation)
                .Include(s => s.Gender)
                .Include(s => s.Specialty)
                .Include(s => s.StudentStates)
                .AsQueryable();

            if (faculty != 0)
            {
                students = students.Where(s => s.Specialty.FacultyID == faculty);

                if (specialty != 0)
                {
                    students = students.Where(s => s.SpecialtyID == specialty);              
                }
            }

            if(formEducation != 0)
            {
                students = students.Where(s => s.FormEducationID == formEducation);
            }

            if (academicDegree != 0)
            {
                students = students.Where(s => s.AcademicDegreeID == academicDegree);
            }

            if(sex != 0)
            {
                students = students.Where(s => s.GenderID == sex);
            }

            if(endYear != 0)
            {
                students = students.Where(s => s.EndDate.Year == endYear);
            }

            var pieChartMain = new PieChart();

            foreach (var employmentStatus in _context.EmploymentStatuses)
            {
                pieChartMain.labels.Add(employmentStatus.Name);
            }
            pieChartMain.labels.Add("неизвестно");

            pieChartMain.datasets[0].backgroundColor.Add("#2ecc71");
            pieChartMain.datasets[0].backgroundColor.Add("#3498db");
            pieChartMain.datasets[0].backgroundColor.Add("#95a5a6");
            pieChartMain.datasets[0].backgroundColor.Add("#c9cbcf");
            pieChartMain.datasets[0].backgroundColor.Add("#3498db");
            pieChartMain.datasets[0].backgroundColor.Add("#95a5a6");

            List<int> employmentValues = GetEmploymentValues(students);

            foreach (var employmentValue in employmentValues)
            {
                pieChartMain.datasets[0].data.Add(employmentValue);
            }

            ViewData["PieChartMain"] = pieChartMain;

            ViewData["Specialties"] = _context.Specialties.Where(s => s.FacultyID == faculty).ToList();
            ViewData["Faculties"] = _context.Faculties.ToList();
            ViewData["FormsEducation"] = _context.FormsEducation.ToList();
            ViewData["AcademicDegrees"] = _context.AcademicDegrees.ToList();
            ViewData["Sexes"] = _context.Genders.ToList();

            return View(await students.ToListAsync());
        }

        private List<int> GetEmploymentValues(IQueryable<Student> students)
        {
            List<int> employmentValues = new List<int>();

            var undefinedStudent = 0;
            var employmentStatuses = (from temp in _context.EmploymentStatuses select temp).ToList();

            foreach (var unused in employmentStatuses)
            {
                employmentValues.Add(0);
            }
            
            foreach (var student in students)
            {
                if (!student.StudentStates!.Any())
                {
                    undefinedStudent++;
                    continue;
                }

                var lastStudentState = student.StudentStates.OrderBy(s=> s.StatusDate).Last();
                for (int index = 0; index < employmentStatuses.Count(); index++)
                {
                    if (lastStudentState.EmploymentStatus.Name == employmentStatuses[index].Name)
                    {
                        employmentValues[index]++;
                    }
                }
            }

            employmentValues.Add(undefinedStudent);

            return employmentValues;
        }

        // GET: Reports/Details/5
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

        // GET: Reports/Create
        public IActionResult Create()
        {
            ViewData["AcademicDegreeID"] = new SelectList(_context.AcademicDegrees, "AcademicDegreeID", "AcademicDegreeID");
            ViewData["FormEducationID"] = new SelectList(_context.FormsEducation, "FormEducationID", "FormEducationID");
            ViewData["GenderID"] = new SelectList(_context.Genders, "GenderID", "GenderID");
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID");
            return View();
        }

        // POST: Reports/Create
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

        // GET: Reports/Edit/5
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

        // POST: Reports/Edit/5
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

        // GET: Reports/Delete/5
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

        // POST: Reports/Delete/5
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

        public ActionResult GetItems(int id)
        {
            var faculties = _context.Specialties.Where(s => s.FacultyID == id);
            return PartialView(faculties);
        }
    }
}
