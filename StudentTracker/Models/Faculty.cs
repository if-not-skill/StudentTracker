using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace StudentTracker.Models
{
    public class Faculty
    {
        public int FacultyID { get; set; }
        public string FacultyName { get; set; }
        public string FacultyShortName { get; set; }
        public string ImageName { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        //navigation properties
        public ICollection<Specialty> Specialties { get; set; }
    }
}
