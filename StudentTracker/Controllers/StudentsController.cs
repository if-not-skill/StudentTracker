using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using StudentTracker.Data;
using StudentTracker.Models;

namespace StudentTracker.Controllers
{
    [Authorize(Roles = "admin, worker")]
    public class StudentsController : Controller
    {
        private readonly StudentTrackerContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;

        public StudentsController(StudentTrackerContext context, IEmailSender emailSender,
            IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _emailSender = emailSender;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["LastNameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewData["FirstNameSortParam"] = sortOrder == "first_name" ? "first_name_desc" : "first_name";
            ViewData["MidNameSortParam"] = sortOrder == "mid_name" ? "mid_name_desc" : "mid_name";
            ViewData["GenderSortParam"] = sortOrder == "gender" ? "gender_desc" : "gender";
            ViewData["EndDateSortParam"] = sortOrder == "end_date" ? "end_date_desc" : "end_date";
            ViewData["FacultySortParam"] = sortOrder == "faculty" ? "faculty_desc" : "faculty";
            ViewData["SpecialtySortParam"] = sortOrder == "specialty" ? "specialty_desc" : "specialty";
            ViewData["AcademicDegreeSortParam"] =
                sortOrder == "academic_degree" ? "academic_degree_desc" : "academic_degree";
            ViewData["FormEducationSortParam"] =
                sortOrder == "form_education" ? "form_education_desc" : "form_education";
            ViewData["CurrentFilter"] = searchString ?? currentFilter;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var students = _context.Students
                .Include(s => s.AcademicDegree)
                .Include(s => s.FormEducation)
                .Include(s => s.Gender)
                .Include(s => s.Specialty)
                .Include(s => s.Specialty.Faculty).OrderBy(s => s.LastName);


            if (!String.IsNullOrEmpty(searchString))
            {
                var searchWords = searchString.ToLower().Split(' ');

                foreach (var searchWord in searchWords)
                {
                    students = (IOrderedQueryable<Student>) students.Where(
                        s => s.FirstName.Contains(searchWord)
                             || s.LastName.Contains(searchWord)
                             || s.MidName.Contains(searchWord)
                             || s.Gender.GenderName.Contains(searchWord)
                             || s.EndDate.Year.ToString().Contains(searchWord)
                             || s.Specialty.Faculty.FacultyShortName.Contains(searchWord)
                             || s.Specialty.SpecialtyName.Contains(searchWord)
                             || s.AcademicDegree.AcademicDegreeName.Contains(searchWord)
                             || s.FormEducation.FormEducationName.Contains(searchWord));
                }
            }

            switch (sortOrder)
            {
                case "last_name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "end_date":
                    students = students.OrderBy(s => s.EndDate);
                    break;
                case "end_date_desc":
                    students = students.OrderByDescending(s => s.EndDate);
                    break;
                case "first_name":
                    students = students.OrderBy(s => s.FirstName);
                    break;
                case "first_name_desc":
                    students = students.OrderByDescending(s => s.FirstName);
                    break;
                case "mid_name":
                    students = students.OrderBy(s => s.MidName);
                    break;
                case "mid_name_desc":
                    students = students.OrderByDescending(s => s.MidName);
                    break;
                case "gender":
                    students = students.OrderBy(s => s.Gender);
                    break;
                case "gender_desc":
                    students = students.OrderByDescending(s => s.Gender);
                    break;
                case "faculty":
                    students = students.OrderBy(s => s.Specialty.Faculty);
                    break;
                case "faculty_desc":
                    students = students.OrderByDescending(s => s.Specialty.Faculty);
                    break;
                case "specialty":
                    students = students.OrderBy(s => s.Specialty);
                    break;
                case "specialty_desc":
                    students = students.OrderByDescending(s => s.Specialty);
                    break;
                case "academic_degree":
                    students = students.OrderBy(s => s.AcademicDegree);
                    break;
                case "academic_degree_desc":
                    students = students.OrderByDescending(s => s.AcademicDegree);
                    break;
                case "form_education":
                    students = students.OrderBy(s => s.FormEducation);
                    break;
                case "form_education_desc":
                    students = students.OrderByDescending(s => s.FormEducation);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;

            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> AllSendMessage(string sortOrder, string currentFilter,
           string searchString, int? pageNumber)
        {
            searchString ??= currentFilter;

            var pathToFile = _hostEnvironment.WebRootPath
                             + Path.DirectorySeparatorChar.ToString()
                             + "templates"
                             + Path.DirectorySeparatorChar.ToString()
                             + "EmailTemplate"
                             + Path.DirectorySeparatorChar.ToString()
                             + "UpdateStudentStatus.html";

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            {
                builder.HtmlBody = await SourceReader.ReadToEndAsync();
            }

            var students = _context.Students
                .Include(s => s.AcademicDegree)
                .Include(s => s.FormEducation)
                .Include(s => s.Gender)
                .Include(s => s.Specialty)
                .Include(s => s.Specialty.Faculty)
                .Include(s => s.StudentStates)
                .OrderBy(s => s.LastName);

            if (!String.IsNullOrEmpty(searchString))
            {
                var searchWords = searchString.ToLower().Split(' ');

                foreach (var searchWord in searchWords)
                {
                    students = (IOrderedQueryable<Student>)students.Where(
                        s => s.FirstName.Contains(searchWord)
                             || s.LastName.Contains(searchWord)
                             || s.MidName.Contains(searchWord)
                             || s.Gender.GenderName.Contains(searchWord)
                             || s.EndDate.Year.ToString().Contains(searchWord)
                             || s.Specialty.Faculty.FacultyShortName.Contains(searchWord)
                             || s.Specialty.SpecialtyName.Contains(searchWord)
                             || s.AcademicDegree.AcademicDegreeName.Contains(searchWord)
                             || s.FormEducation.FormEducationName.Contains(searchWord));
                }
            }

            var Now = DateTime.Now;

            foreach (var student in students)
            {
                if (!student.StudentStates.Any() || GetLastStatesDate(student).AddMonths(1) < Now)
                {
                    var updateLink = this.Url.Action("UpdateStudentStatus", "Students",
                        new {id = student.StudentID, firstName = student.FirstName, lastName = student.LastName},
                        Url.ActionContext.HttpContext.Request.Scheme);

                    var messageContent = builder.HtmlBody;
                    messageContent = messageContent.Replace("UpdateLink", updateLink);
                    messageContent = messageContent.Replace("LastName", student.LastName);
                    messageContent = messageContent.Replace("FirstName", student.FirstName);

                    var message = new Message(new string[] {student.EmailAddress},
                        "Обновление информации о трудоустройстве",
                        messageContent);
                    await _emailSender.SendEmailAsync(message);
                }
            }

            return RedirectToAction("Index",
                new
                {
                    sortOrder = sortOrder,
                    currentFilter = currentFilter,
                    searchString = searchString,
                    pageNumber = pageNumber
                });
        }

        private DateTime GetLastStatesDate(Student student)
        {
            DateTime dateTime = new DateTime();

            foreach (var studentState in student.StudentStates)
            {
                if (dateTime < studentState.StatusDate)
                {
                    dateTime = studentState.StatusDate;
                }
            }

            return dateTime;
        }

        public async Task<IActionResult> SendMessage(int? id, string sortOrder, string currentFilter,
            string searchString, int? pageNumber)
        {
            if (id == null) return NotFound();

            var student = _context.Students.First(s => s.StudentID == id);

            var pathToFile = _hostEnvironment.WebRootPath
                             + Path.DirectorySeparatorChar.ToString()
                             + "templates"
                             + Path.DirectorySeparatorChar.ToString()
                             + "EmailTemplate"
                             + Path.DirectorySeparatorChar.ToString()
                             + "UpdateStudentStatus.html";

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            var updateLink = this.Url.Action("UpdateStudentStatus", "Students",
                new {id = student.StudentID, firstName = student.FirstName, lastName = student.LastName},
                Url.ActionContext.HttpContext.Request.Scheme);

            var messageContent = builder.HtmlBody;
            messageContent = messageContent.Replace("UpdateLink", updateLink);
            messageContent = messageContent.Replace("LastName", student.LastName);
            messageContent = messageContent.Replace("FirstName", student.FirstName);

            var message = new Message(new string[] {student.EmailAddress}, "Обновление информации о трудоустройстве",
                messageContent);
            await _emailSender.SendEmailAsync(message);

            return RedirectToAction("Index",
                new
                {
                    sortOrder = sortOrder,
                    currentFilter = currentFilter,
                    searchString = searchString,
                    pageNumber = pageNumber
                });
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
                .Include(s => s.Specialty.Faculty)
                .Include(s => s.Specialty)
                .FirstOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }

            var studentStates = new List<StudentState>(_context.StudentStates
                .Where(s => s.StudentID == student.StudentID)
                .Include(s => s.EmploymentStatus)
                .OrderBy(s => s.StatusDate));

            ViewData["StudentStates"] = studentStates;

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["AcademicDegreeID"] =
                new SelectList(_context.AcademicDegrees, "AcademicDegreeID", "AcademicDegreeName");
            ViewData["FormEducationID"] =
                new SelectList(_context.FormsEducation, "FormEducationID", "FormEducationName");
            ViewData["GenderID"] = new SelectList(_context.Genders, "GenderID", "GenderName");

            int selectedIndex = 1;
            SelectList faculties = new SelectList(_context.Faculties, "FacultyID", "FacultyName", selectedIndex);
            ViewData["Faculties"] = faculties;
            SelectList specialties = new SelectList(_context.Specialties.Where(c => c.FacultyID == selectedIndex),
                "SpecialtyID", "SpecialtyName");
            ViewData["SpecialtyID"] = specialties;

            return View();
        }

        public ActionResult GetItems(int id)
        {
            return PartialView(_context.Specialties.Where(s => s.FacultyID == id));
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "StudentID,LastName,FirstName,MidName,GenderID,PhoneNumber,EmailAddress,Faculty,SpecialtyID,FormEducationID,AcademicDegreeID,IsHasRedDiploma,EndDate")]
            Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AcademicDegreeID"] = new SelectList(_context.AcademicDegrees, "AcademicDegreeID",
                "AcademicDegreeID", student.AcademicDegreeID);
            ViewData["FormEducationID"] = new SelectList(_context.FormsEducation, "FormEducationID",
                "FormEducationName", student.FormEducationID);
            ViewData["GenderID"] = new SelectList(_context.Genders, "GenderID", "GenderID", student.GenderID);
            ViewData["SpecialtyID"] =
                new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", student.SpecialtyID);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Specialty)
                .Include(s => s.Specialty.Faculty)
                .FirstOrDefaultAsync(s => s.StudentID == id.Value);

            if (student == null)
            {
                return NotFound();
            }

            ViewData["AcademicDegreeID"] = new SelectList(_context.AcademicDegrees, "AcademicDegreeID",
                "AcademicDegreeName", student.AcademicDegreeID);
            ViewData["FormEducationID"] = new SelectList(_context.FormsEducation, "FormEducationID",
                "FormEducationName", student.FormEducationID);
            ViewData["GenderID"] = new SelectList(_context.Genders, "GenderID", "GenderName", student.GenderID);

            if (student.Specialty != null)
            {
                SelectList faculties = new SelectList(_context.Faculties, "FacultyID", "FacultyName",
                    student.Specialty.FacultyID);
                ViewData["Faculties"] = faculties;
            }

            SelectList specialties =
                new SelectList(_context.Specialties.Where(s => s.FacultyID == student.Specialty.FacultyID),
                    "SpecialtyID", "SpecialtyName", student.SpecialtyID);
            ViewData["Specialties"] = specialties;


            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind(
                "StudentID,LastName,FirstName,MidName,GenderID,PhoneNumber,EmailAddress,SpecialtyID,FormEducationID,AcademicDegreeID,IsHasRedDiploma,EndDate")]
            Student student)
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

            ViewData["AcademicDegreeID"] = new SelectList(_context.AcademicDegrees, "AcademicDegreeID",
                "AcademicDegreeID", student.AcademicDegreeID);
            ViewData["FormEducationID"] = new SelectList(_context.FormsEducation, "FormEducationID", "FormEducationID",
                student.FormEducationID);
            ViewData["GenderID"] = new SelectList(_context.Genders, "GenderID", "GenderID", student.GenderID);
            ViewData["SpecialtyID"] =
                new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", student.SpecialtyID);
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

        [AllowAnonymous]
        public IActionResult UpdateStudentStatus(int? id, string firstName, string lastName)
        {
            if (id == null || firstName == null || lastName == null)
            {
                return NotFound();
            }

            var student = _context.Students.First(s => s.StudentID == id);



            if (student == null || student.FirstName != firstName || student.LastName != lastName)
            {
                return NotFound();
            }

            ViewData["StudentID"] = id;
            ViewData["EmploymentStatusID"] = new SelectList(_context.EmploymentStatuses, "EmploymentStatusID", "Name");
            ViewData["Student"] = student;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateStudentStatus(
            [Bind("StudentStateID,StudentID,EmploymentStatusID,CountryName,CityName")]
            StudentState studentState)
        {
            if (ModelState.IsValid)
            {
                studentState.StatusDate = DateTime.Now;

                _context.Add(studentState);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(EndUpdate));
            }

            ViewData["EmploymentStatusID"] = new SelectList(_context.EmploymentStatuses, "EmploymentStatusID",
                "EmploymentStatusID", studentState.EmploymentStatusID);
            ViewData["StudentID"] =
                new SelectList(_context.Students, "StudentID", "EmailAddress", studentState.StudentID);
            return View();
        }

        [AllowAnonymous]
        public IActionResult EndUpdate()
        {
            return View();
        }
    }
}
