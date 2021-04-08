using System;
using System.Collections.Generic;

namespace StudentTracker.Models
{
    public enum AcademicDegrees
    {
        Baccalaureate,
        Specialty,
        MastersDegree,
        GraduateStudent
    }

    public class Student
    {
        public int StudentID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MidName { get; set; }
        public int SpecialtyID { get; set; }
        public AcademicDegrees AcademicDegree { get; set; }
        public DateTime EndDate { get; set; }

        //navigation properties
        public ICollection<StudentState> StudentStates { get; set; }
        public Specialty Specialty { get; set; }

    }
}
