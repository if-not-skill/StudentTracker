using StudentTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            var Students = new Student[]
            {
                new Student{ LastName = "Иванов", FirstName="Иван", MidName="Иванов", SpecialtyID=1, AcademicDegree=AcademicDegrees.Baccalaureate},
                new Student{ LastName = "Петров", FirstName="Иван", MidName="Иванов", SpecialtyID=2, AcademicDegree=AcademicDegrees.Baccalaureate},
                new Student{ LastName = "Соболев", FirstName="Иван", MidName="Иванов", SpecialtyID=1, AcademicDegree=AcademicDegrees.Baccalaureate},
            };

            foreach(var Student in Students)
            {
                context.Students.Add(Student);
            }
            context.SaveChanges();

            var Faculties = new Faculty[]
            {
                new Faculty { FacultyName="Факультет Менеджмента и предпринимательства", FacultyShortName="МиП"},
                new Faculty { FacultyName="Факультет Торгового дела", FacultyShortName="ТД"},
                new Faculty { FacultyName="Факультет Компьютерных технологий и информационной безопасности", FacultyShortName="КТиИБ"},
                new Faculty { FacultyName="Учетно-экономический факультет", FacultyShortName="УЭФ"},
                new Faculty { FacultyName="Факультет Экономики и финансов", FacultyShortName="ЭиФ"},
                new Faculty { FacultyName="Юридический факультет", FacultyShortName="ЮФ"},
                new Faculty { FacultyName="Факультет Лингвистики и журналистики", FacultyShortName="ЛиЖ"}
            };

            foreach(var Faculty in Faculties)
            {
                context.Faculties.Add(Faculty);
            }
            context.SaveChanges();

            var Specialties = new Specialty[] 
            {
                new Specialty{SpecialtyName="Прикладная математика и информатика", FacultyID=3},
                new Specialty{SpecialtyName="Программная инженерия", FacultyID=3},
            };

            foreach(var Specialty in Specialties)
            {
                context.Specialties.Add(Specialty);
            }
            context.SaveChanges();
            
        }
    }
}
