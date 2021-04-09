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

        //navigation properties
        public Student Student { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
    }
}
