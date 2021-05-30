using StudentTracker.Models;
using System;
using System.Linq;

namespace StudentTracker.Data
{
    public class DbInitializer
    {
        public static void Initialize(StudentTrackerContext context)
        {
            context.Database.EnsureCreated();

            if (context.Students.Any())
            {
                return;
            }

            //Genders
            var genders = new Gender[]
            {
                new Gender{GenderName = "Мужчина"},
                new Gender{GenderName = "Женщина"}
            };

            foreach (var gender in genders)
            {
                context.Genders.Add(gender);
            }
            context.SaveChanges();


            //Employment Statuses
            var employmentStatuses = new EmploymentStatus[]
            {
                new EmploymentStatus{Name = "работает"},
                new EmploymentStatus{Name = "работает по специальности"},
                new EmploymentStatus{Name = "не работает"}
            };

            foreach (var employmentStatus in employmentStatuses)
            {
                context.EmploymentStatuses.Add(employmentStatus);
            }
            context.SaveChanges();


            //Forms Educations
            var formsEducation = new FormEducation[]
            {
                new FormEducation {FormEducationName = "очно"},
                new FormEducation {FormEducationName = "очно-заочно"},
                new FormEducation {FormEducationName = "заочно"}
            };

            foreach (var formEducation in formsEducation)
            {
                context.FormsEducation.Add(formEducation);
            }
            context.SaveChanges();


            //AcademicDegrees
            var academicDegrees = new AcademicDegree[]
            {
                new AcademicDegree{AcademicDegreeName = "Бакалавриат"},
                new AcademicDegree{AcademicDegreeName = "Специалитет"},
                new AcademicDegree{AcademicDegreeName = "Магистратура"},
                new AcademicDegree{AcademicDegreeName = "Аспирантура"}
            };

            foreach (var academicDegree in academicDegrees)
            {
                context.AcademicDegrees.Add(academicDegree);
            }
            context.SaveChanges();


            //Faculties
            var faculties = new Faculty[]
            {
                new Faculty { FacultyName="Факультет Менеджмента и предпринимательства", FacultyShortName="МиП", ImageName = "mip.png"},
                new Faculty { FacultyName="Факультет Компьютерных технологий и информационной безопасности", FacultyShortName="КТиИБ", ImageName = "ktib.png"},
                new Faculty { FacultyName="Факультет Торгового дела", FacultyShortName="ТД", ImageName = "td.png"},
                new Faculty { FacultyName="Учетно-экономический факультет", FacultyShortName="УЭФ", ImageName = "uef.png"},
                new Faculty { FacultyName="Факультет Экономики и финансов", FacultyShortName="ЭиФ", ImageName = "eif.png"},
                new Faculty { FacultyName="Юридический факультет", FacultyShortName="ЮФ", ImageName = "urf.png"},
                new Faculty { FacultyName="Факультет Лингвистики и журналистики", FacultyShortName="ЛиЖ", ImageName = "lig.png"}
            };

            foreach(var faculty in faculties)
            {
                context.Faculties.Add(faculty);
            }
            context.SaveChanges();


            //Specialties
            var specialties = new Specialty[] 
            {
                new Specialty{SpecialtyName="Прикладная математика и информатика", FacultyID=2},
                new Specialty{SpecialtyName="Программная инженерия", FacultyID=2},
            };

            foreach(var specialty in specialties)
            {
                context.Specialties.Add(specialty);
            }
            context.SaveChanges();
            

            //Students
            var students = new Student[]
            {
                new Student{ LastName = "Иванов", FirstName="Иван", MidName="Иванов", GenderID = 1, PhoneNumber = "89518400782", EmailAddress = "regdayz@gmail.com", SpecialtyID=1, AcademicDegreeID = 1, IsHasRedDiploma = false, FormEducationID = 1, EndDate = DateTime.Now},
                new Student{ LastName = "Петров", FirstName="Иван", MidName="Иванов", GenderID = 1, PhoneNumber = "89518400782", EmailAddress = "regdayz@gmail.com",  SpecialtyID=2, AcademicDegreeID = 1, IsHasRedDiploma = false, FormEducationID = 1, EndDate = DateTime.Now},
                new Student{ LastName = "Соболев", FirstName="Иван", MidName="Иванов", GenderID = 1, PhoneNumber = "89518400782", EmailAddress = "regdayz@gmail.com", SpecialtyID=1, AcademicDegreeID = 1, IsHasRedDiploma = false, FormEducationID = 1, EndDate = DateTime.Now},
            };

            foreach (var student in students)
            {
                context.Students.Add(student);
            }
            context.SaveChanges();

        }
    }
}
