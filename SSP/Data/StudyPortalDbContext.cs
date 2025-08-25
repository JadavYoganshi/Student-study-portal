using Microsoft.EntityFrameworkCore;
using SSP.Models.Domain;

namespace SSP.Data
{
    public class StudyPortalDbContext : DbContext
    {
        public StudyPortalDbContext(DbContextOptions<StudyPortalDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Record> Records { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure ID's are not auto-generated for certain entities
            modelBuilder.Entity<Student>()
                .Property(s => s.S_Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Admin>()
                .Property(a => a.A_Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Homework>()
                .HasKey(h => h.HomeworkId);

            modelBuilder.Entity<Homework>()
                .HasOne(h => h.Student)
                .WithMany(s => s.Homeworks)
                .HasForeignKey(h => h.S_Id);

            modelBuilder.Entity<Todo>()
                .HasOne(t => t.Student)
                .WithMany(s => s.Todos)
                .HasForeignKey(t => t.S_Id)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if necessary

            modelBuilder.Entity<Todo>()
                .Property(t => t.S_Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Record>()
                .HasKey(r => r.R_Id);

            // Seed data for default student and admin
            modelBuilder.Entity<Student>().HasData(new Student
            {
                S_Id = new Guid("3D6FA47D-1F9D-4D77-8E2D-F20C2584EDDA"),
                S_Name = "Test Student",
                S_Email = "test@example.com",
                S_Password = "hashedpassword" // Store hashed password, not plain text
            });

            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                A_Id = Guid.Parse("b7c9fbb0-04f9-4a62-8925-493f2d4377e2"),
                A_Name = "Admin User",
                A_Email = "admin@example.com",
                A_Password = "hashedpassword" // Store hashed password, not plain text
            });
        }
    }
}
