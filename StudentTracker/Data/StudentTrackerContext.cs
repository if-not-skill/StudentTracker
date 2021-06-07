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
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role adminRole = new Role {Id = 1, Name = "admin"}; 
            Role workerRole = new Role { Id = 2, Name = "worker" };
            Role userRole = new Role { Id = 3, Name = "user" };

            User adminUser = new User {UserId = 1, Email = "admin", Password = "87654321", RoleId = adminRole.Id};

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, workerRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });

            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<StudentState>().ToTable("StudentState");
            modelBuilder.Entity<Specialty>().ToTable("Specialty");
            modelBuilder.Entity<Faculty>().ToTable("Faculty");
            modelBuilder.Entity<AcademicDegree>().ToTable("AcademicDegree");
            modelBuilder.Entity<FormEducation>().ToTable("FormEducation");
            modelBuilder.Entity<EmploymentStatus>().ToTable("EmploymentStatus");
            modelBuilder.Entity<Gender>().ToTable("Gender");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");

            base.OnModelCreating(modelBuilder);
        }
    }
}
