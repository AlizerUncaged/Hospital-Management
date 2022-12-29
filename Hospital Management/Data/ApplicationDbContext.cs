using Hospital_Management.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Patient> Patients { get; set; }

    public DbSet<Dentist> Dentists { get; set; }
    
    public DbSet<MedicinePayment> MedicinePayments { get; set; }

    public DbSet<AppointmentModel> Appointments { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}