using Microsoft.EntityFrameworkCore;
using Doctor_CLinic_API.Models;
using Doctor_CLinic_API.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Doctor_CLinic_API.Data
{
    public class appContext : IdentityDbContext<User , Role , int>
    {
        public appContext(DbContextOptions<appContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationship between User and Patient
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany(u => u.Patients)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many relationship between Patient and Appointment
            modelBuilder.Entity<Appointment>()
                   .HasOne(a => a.Patient)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship between Appointment and User
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments) 
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
