namespace StudentTracker.Models
{
    public class Specialty
    {
        public int SpecialtyID { get; set; }
        public string SpecialtyName { get; set; }
        public int FacultyID { get; set; }

        //navigation properties
        public Faculty Faculty { get; set; }
    }
}
