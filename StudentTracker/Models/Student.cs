#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentTracker.Models
{
    public class Student
    {
        public int StudentID { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }
        public string MidName { get; set; }
        public int GenderID { get; set; } = 1;
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public int SpecialtyID { get; set; } = 1;
        public int FormEducationID { get; set; } = 1;
        public int AcademicDegreeID { get; set; } = 1;
        public bool IsHasRedDiploma { get; set; } = false;
        public DateTime EndDate { get; set; }

        //navigation properties
        public ICollection<StudentState>? StudentStates { get; set; }
        public Specialty? Specialty { get; set; }
        public AcademicDegree? AcademicDegree { get; set; }
        public FormEducation? FormEducation { get; set; }
        public Gender? Gender { get; set; }

    }
}
