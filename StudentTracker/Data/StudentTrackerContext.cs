using Microsoft.EntityFrameworkCore;
using StudentTracker.Models;

namespace StudentTracker.Data
{
    public class StudentTrackerContext : DbContext
    {
        public StudentTrackerContext(DbContextOptions<StudentTrackerContext> options) : base(options){}

        public DbSet<Student> Students { get; set; }
        public DbSet<StudentState> StudentStates { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<AcademicDegree> AcademicDegrees { get; set; }
        public DbSet<FormEducation> FormsEducation { get; set; }
        public DbSet<EmploymentStatus> EmploymentStatuses { get; set; }
        public DbSet<Gender> Genders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<StudentState>().ToTable("StudentState");
            modelBuilder.Entity<Specialty>().ToTable("Specialty");
            modelBuilder.Entity<Faculty>().ToTable("Faculty");
            modelBuilder.Entity<AcademicDegree>().ToTable("AcademicDegree");
            modelBuilder.Entity<FormEducation>().ToTable("FormEducation");
            modelBuilder.Entity<EmploymentStatus>().ToTable("EmploymentStatus");
            modelBuilder.Entity<Gender>().ToTable("Gender");
        }
    }
}
