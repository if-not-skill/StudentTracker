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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<StudentState>().ToTable("StudentState");
            modelBuilder.Entity<Specialty>().ToTable("Specialty");
            modelBuilder.Entity<Faculty>().ToTable("Faculty");
        }
    }
}
