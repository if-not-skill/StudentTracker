using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentTracker.Models
{
    public class StudentState
    {
        public int StudentStateID { get; set; }
        public int StudentID { get; set; }
        public int EmploymentStatusID { get; set; }
        public DateTime StatusDate { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string OrganizationName { get; set; }
        public string PostName { get; set; }

        //navigation properties
        public Student Student { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
    }
}
