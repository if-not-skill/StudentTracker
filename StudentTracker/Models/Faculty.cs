using System.Collections.Generic;

namespace StudentTracker.Models
{
    public class Faculty
    {
        public int FacultyID { get; set; }
        public string FacultyName { get; set; }
        public string FacultyShortName { get; set; }

        //navigation properties
        public ICollection<Specialty> Specialties { get; set; }
    }
}
